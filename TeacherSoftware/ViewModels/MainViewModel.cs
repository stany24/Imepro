using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ClassLibrary6.Command;
using ClassLibrary6.Data;
using ClassLibrary6.History;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using TeacherSoftware.Logic;
using TeacherSoftware.Views;

namespace TeacherSoftware.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public readonly Main MainWindow;
    private readonly List<IndividualStudentWindow> DisplayStudents = [];
    private ConfigurationWindow? _configurationWindow;
    private StreamOptionsWindow? _streamOptionsWindow;
    
    [ObservableProperty] private bool _isEnabled = true;
    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private string _lblIpText = "";
    [ObservableProperty] private string _btnShareContent = "Share";
    [ObservableProperty] private string _btnFilterContent = "Filter";
    [ObservableProperty] private List<Preview> _previews = [];
    [ObservableProperty] private List<string> _allInfos = [];
    [ObservableProperty] private List<string> _requests = [];
    [ObservableProperty] private List<string> _connections = [];
    
    private IPAddress ipAddr;
    private int NextId;
    private bool isAsking;
    
    public ObservableCollection<DataForTeacher> Students { get; set; } = [];

    [ObservableProperty] private int _sliderZoomValue = 50;

    public MainViewModel()
    {
        MainWindow = new Main
        {
            DataContext = this
        };
        MainWindow.Closing+=(_,_) => Closing();
        MainWindow.Show();
        FindIp();
        Task.Run(AskStudentsForData);
        Task.Run(LogClients);
        FindIp();
    }

    private void FindIp()
    {
        // Establish the local endpointfor the socket.
        // Dns.GetHostName returns the name of the host running the application.
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> possiblesIp = ipHost.AddressList.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToList();
        possiblesIp.RemoveAll(x => x.Equals(IPAddress.Parse("127.0.1.1")));
        switch (possiblesIp.Count)
        {
            case 0:
                Console.WriteLine("No IP address found"); // TODO : add message box and close app
                break;
            case 1: 
                ipAddr = possiblesIp[0];
                Console.WriteLine("IP address found: " + ipAddr);
                break;
            default:
                Console.WriteLine("Multiple IP address found"); // TODO : add window to choose the right one
                break;
        }
        LblIpText = "IP: " + ipAddr;
    }

    private void LogClients()
    {
        IPEndPoint localEndPoint = new(ipAddr, 11111);
        Socket listener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(-1);
        while (true)
        {
            try
            {
                Socket clientSocket = listener.Accept();
                Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " Nouvelle connexion de: " + clientSocket.RemoteEndPoint);
                Students.Add(new DataForTeacher(clientSocket, NextId));
                Task.Run(() => SendAuthorisedUrl(clientSocket));
                NextId++;
            }
            catch (Exception e) { Connections.Add(e.ToString()); }
        }
    }

    private void SendAuthorisedUrl(Socket socket)
    {
        while (isAsking) { Thread.Sleep(100); }
        isAsking = true;
        socket.Send(new Command(CommandType.ReceiveAutorisedUrls).ToByteArray());
        //serialization
        string content = JsonSerializer.Serialize(new List<string> { "youtube.com", "github.com" });//TODO : add to settings
        string jsonString = JsonSerializer.Serialize(new Message(CommandType.ReceiveAutorisedUrls, content));
        //envoi
        Thread.Sleep(100);
        socket.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
        isAsking = false;
    }
    
    private void AskStudentsForData()
    {
        while (true)
        {
            if (Students.Count != 0)
            {
                while (isAsking) { Thread.Sleep(10); }
                isAsking = true;
                DateTime startUpdate = DateTime.Now;
                DateTime nextUpdate = DateTime.Now.AddSeconds(15); //TODO : add to settings
                List<DataForTeacher> clientToRemove = [];
                UpdateStudent(clientToRemove);
                foreach (DataForTeacher client in clientToRemove)
                {
                    RemoveStudent(client);
                }
                UpdateAllIndividualDisplay();
                DateTime finishedUpdate = DateTime.Now;
                TimeSpan updateDuration = finishedUpdate - startUpdate;
                TimeSpan cycleDuration = nextUpdate - startUpdate;
                if (cycleDuration <= updateDuration) continue;
                isAsking = false;
                Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " Attente du prochain cycle dans " + (cycleDuration - updateDuration) + " secondes");
                Thread.Sleep(cycleDuration - updateDuration);
            }
            else { Thread.Sleep(100); }
        }
    }

    private void UpdateAllIndividualDisplay()
    {
        foreach (IndividualStudentWindow display in DisplayStudents)
        {
            foreach (DataForTeacher student in Students)
            {
                if (display.GetStudentId() == student.Id)
                {
                    display.UpdateDisplay(student);
                }
            }
        }
    }

    private void UpdateStudent(List<DataForTeacher> clientToRemove)
    {
        for (int i = 0; i < Students.Count; i++)
        {
            Socket socket = Students[i].SocketToStudent;
            socket.ReceiveTimeout = 1; //TODO : add to settings
            socket.SendTimeout = 1; //TODO : add to settings
            try
            {
                socket.Send(new Command(CommandType.DemandData).ToByteArray());
                Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande des données à " + Students[i].UserName);
                Task.Run(() => Students[i] = ReceiveData(Students[i])).Wait();
                socket.Send(new Command(CommandType.DemandImage).ToByteArray());
                Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande de l'image à " + Students[i].UserName);
                Task.Run(() => ReceiveImage(Students[i])).Wait();
            }
            catch (SocketException)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                clientToRemove.Add(Students[i]);
            }
        }
    }
    
    private DataForTeacher ReceiveData(DataForTeacher student)
    {
        try
        {
            Socket socket = student.SocketToStudent;
            int id = student.Id;
            byte[] dataBuffer = new byte[100000];
            socket.ReceiveTimeout = 1; //TODO : add to settings
            int nbData = socket.Receive(dataBuffer);
            Array.Resize(ref dataBuffer, nbData);
            Data data = JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer));
            student = new DataForTeacher(data)
            {
                SocketToStudent = socket,
                Id = id
            };
            Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " Données recue de " + student.UserName);
            student.NumberOfFailure = 0;
            return student;
        }
        catch
        {
            Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé de donnée");
            student.NumberOfFailure++;
            return student;
        }
    }
    
    private void ReceiveImage(DataForTeacher student)
    {
        try
        {
            Socket socket = student.SocketToStudent;
            byte[] imageBuffer = new byte[10485760];
            socket.ReceiveTimeout = 1; //TODO : add to settings
            int nbData = socket.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
            Array.Resize(ref imageBuffer, nbData);
            student.ScreenShot = new MagickImage(new MemoryStream(imageBuffer));
            Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " Image recue de " + student.UserName);
            student.NumberOfFailure = 0;
            Previews.Add(new Preview(student.Id, student.ComputerName, new Bitmap(new MemoryStream(imageBuffer))));
        }
        catch
        {
            Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé d'image");
            student.NumberOfFailure++;
        }
    }

    private void RemoveStudent(DataForTeacher student)
    {
        Students.Remove(student);
        Requests.Add(DateTime.Now.ToString("HH:mm:ss") + " L'élève " + student.UserName + " est déconnecté");
    }

    private void Closing()
    {
        foreach (IndividualStudentWindow displayStudent in DisplayStudents)
        {
            displayStudent.Close();
        }
        _configurationWindow?.Close();
        _streamOptionsWindow?.Close();
    }
    
    public void OpenConfigurationWindow()
    {
        _configurationWindow = new ConfigurationWindow
        {
            DataContext = this
        };
        _configurationWindow.Show();
    }
    
    public void OpenStreamOptionsWindow()
    {
        _streamOptionsWindow = new StreamOptionsWindow
        {
            DataContext = this
        };
        _streamOptionsWindow.Show();
    }
}