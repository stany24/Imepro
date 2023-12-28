using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using IronSoftware.Drawing;
using Library;

namespace TeacherApp;

public partial class MainWindow : Window
{
    readonly private PreviewDisplayer Displayer;
    readonly private List<DataForTeacher> AllStudents = new();
    readonly private List<DisplayStudent> AllStudentsDisplay = new();

    private List<DataForTeacher> StudentToShareScreen = new();
    private Task ScreenSharer;
    private bool isSharing = false;
    private bool isAsking;
    private int NextId;
    private IPAddress ipAddr;
    private ReliableMulticastSender MulticastSender;
    
    public MainWindow()
    {
        InitializeComponent();
        FindIp();
        Task.Run(AskingData);
        Task.Run(LogClients);
    }

    private void FindIp()
    {
        // Establish the local endpointfor the socket.
        // Dns.GetHostName returns the name of the host running the application.
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> possiblesIp = ipHost.AddressList.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToList();
        switch (possiblesIp.Count)
        {
            case 0:
                MessageBox.Show("Aucune addresse ip conforme n'a étée trouvée.\r\n" +
                                "Vérifiez vos connexion aux réseaux.\r\n" +
                                "L'application va ce fermer.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                break;
            case 1: ipAddr = possiblesIp[0]; break;
            default:
                AskToChoseIp prompt = new(possiblesIp);
                if (prompt.ShowDialog(this) == DialogResult.OK) { ipAddr = prompt.GetChoosenIp(); }
                else { ipAddr = possiblesIp[0]; }
                prompt.Dispose();
                break;
        }
        lblIP.Text = "IP: " + ipAddr;
    }
    
    private void AskingData()
    {
        while (true)
        {
            if (AllStudents.Count != 0)
            {
                while (isAsking) { Thread.Sleep(10); }
                isAsking = true;
                DateTime startUpdate = DateTime.Now;
                DateTime nextUpdate = DateTime.Now.AddSeconds(15);
                List<DataForTeacher> clientToRemove = new();
                UpdateStudents(clientToRemove);
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
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Attente du prochain cycle dans " + (cycleDuration - updateDuration) + " secondes");
                });
                Thread.Sleep(cycleDuration - updateDuration);
            }
            else { Thread.Sleep(100); }
        }
    }

    private void UpdateStudents(ICollection<DataForTeacher> clientToRemove)
    {
        for (int i = 0; i < AllStudents.Count; i++)
        {
            Socket socket = AllStudents[i].SocketToStudent;
            socket.ReceiveTimeout = Properties.Settings.Default.DefaultTimeout;
            socket.SendTimeout = Properties.Settings.Default.DefaultTimeout;
            try
            {
                socket.Send(new Command(CommandType.DemandData).ToByteArray() as byte[] ?? Array.Empty<byte>());
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande des données à " + AllStudents[i].UserName);
                });
                int i1 = i;
                Task.Run(() => AllStudents[i1] = ReceiveData(AllStudents[i1])).Wait();
                socket.Send(new Command(CommandType.DemandImage).ToByteArray() as byte[] ?? Array.Empty<byte>());
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande de l'image à " + AllStudents[i].UserName);
                });
                Task.Run(() => ReceiveImage(AllStudents[i])).Wait();
            }
            catch (SocketException)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                clientToRemove.Add(AllStudents[i]);
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
            socket.ReceiveTimeout = Properties.Settings.Default.DefaultTimeout;
            int nbData = socket.Receive(dataBuffer);
            Array.Resize(ref dataBuffer, nbData);
            Data? data = JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer));
            student = new DataForTeacher(data)
            {
                SocketToStudent = socket,
                Id = id
            };
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Données recue de " + student.UserName);
            });
            Task.Run(() => UpdateTreeViews(student));
            student.NumberOfFailure = 0;
            return student;
        }
        catch
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé de donnée");
            });
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
            socket.ReceiveTimeout = Properties.Settings.Default.DefaultTimeout;
            int nbData = socket.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
            Array.Resize(ref imageBuffer, nbData);
            student.ScreenShot = new AnyBitmap(new MemoryStream(imageBuffer));
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Image recue de " + student.UserName);
            });
            student.NumberOfFailure = 0;
            Displayer.UpdatePreview(student.Id, student.ComputerName, student.ScreenShot);
        }
        catch
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé d'image");
            });
            student.NumberOfFailure++;
        }
    }

    private void RemoveStudent(DataForTeacher student)
    {
        AllStudents.Remove(student);
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " L'élève " + student.UserName + " est déconnecté");
        });
        TreeViewDetails.Invoke(new MethodInvoker(delegate
        {
            TreeNode[] nodes = TreeViewDetails.Nodes.Find(Convert.ToString(student.Id), false);
            if (nodes.Any()) { nodes[0].Remove(); }
        }));
        TreeViewSelect.Invoke(new MethodInvoker(delegate
        {
            TreeNode[] nodes = TreeViewDetails.Nodes.Find(Convert.ToString(student.Id), false);
            if (nodes.Any()) { nodes[0].Remove(); }
        }));
    }

    private void UpdateAllIndividualDisplay()
    {
        foreach (DisplayStudent display in AllStudentsDisplay)
        {
            foreach (DataForTeacher student in AllStudents.Where(student => display.GetStudentId() == student.Id))
            {
                display.UpdateAffichage(student);
            }
        }
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
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LbxRequests.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Nouvelle connexion de: " + clientSocket.RemoteEndPoint);
                });
                AllStudents.Add(new DataForTeacher(clientSocket, NextId));
                Task.Run(() => SendAuthorisedUrl(clientSocket));
                NextId++;
            }
            catch (Exception e)
            {
                Dispatcher.UIThread.InvokeAsync(() => { LbxRequests.Items.Add(e.ToString()); });
            }
        }
    }

    private void SendAuthorisedUrl(Socket socket)
    {
        while (isAsking) { Thread.Sleep(100); }
        isAsking = true;
        socket.Send(new Command(CommandType.ReceiveAutorisedUrls).ToByteArray() as byte[] ?? Array.Empty<byte>());
        //serialization
        string jsonString = JsonSerializer.Serialize(Properties.Settings.Default.AutorisedWebsite);
        //envoi
        Thread.Sleep(100);
        socket.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
        isAsking = false;
    }
}