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
using ClassLibrary6.Command;
using ClassLibrary6.Data;
using ClassLibrary6.ReliableMulticast;
using ImageMagick;
using MsBox.Avalonia;
using TeacherSoftware.Logic;
using TeacherSoftware.Logic.Nodes;
using TeacherSoftware.ViewModels;
// ReSharper disable StringLiteralTypo

namespace TeacherSoftware.Views;

public partial class Main : Window
{
    #region Variables
    private readonly List<DataForTeacher> _allStudents = new();
    private readonly List<DisplayStudent> _allStudentsDisplay = new();
    private readonly Properties _properties = new();
    private bool _running = true;

    private List<DataForTeacher> _studentToShareScreen = new();
    private Task _screenSharer;
    private bool _isSharing;
    private bool _isAsking;
    private int _nextId;
    private IPAddress _ipAddr;
    private ReliableMulticastSender _multicastSender;

    private ChooseIp _chooseIpWindow;

    #endregion

    #region At start

    public Main(MainViewModel model)
    {
        DataContext = model;
        InitializeComponent();
        if (FindIp())
        {
            Task.Run(StartTasks);
        }
        Slider.ValueChanged += (_,e) => PreviewDisplay.Zoom = (int)e.NewValue;
        BtnFilter.Click += (_,_) => ButtonFilter_Click();
        BtnShare.Click += (_,_) => ShareScreen();
        BtnHideTreeView.Click += (_, _) => CloseAllTreeViewNodes();
        BtnShowTreeView.Click += (_, _) => OpenAllTreeViewNodes();
        BtnOpenConfiguration.Click += (_, _) => OpenConfiguration();
        Closing += (_, _) => OnClosing();
    }

    private void ChooseIpWindowClosing()
    {
        Show();
        IsEnabled = true;
        _ipAddr = _chooseIpWindow.GetChoosenIp();
        LblIp.Text = "IP: " + _ipAddr;
        Task.Run(StartTasks);
    }

    /// <summary>
    /// Function that starts all tasks running in backgrounds.
    /// </summary>
    private void StartTasks()
    {
        Task.Run(AskingData);
        Task.Run(LogClients);
    }

    /// <summary>
    /// Function that find the teacher ip.
    /// </summary>
    private bool FindIp()
    {
        // Establish the local endpoint for the socket.
        // Dns.GetHostName returns the name of the host running the application.
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> possiblesIp = ipHost.AddressList.Where(address => address.AddressFamily == AddressFamily.InterNetwork).ToList();
        switch (possiblesIp.Count)
        {
            case 0:
                MessageBoxManager.GetMessageBoxStandard(
                    "Attention",
                    "Aucune addresse ip conforme n'a étée trouvée. Vérifiez vos connexion aux réseaux. L'application va ce fermer.");
                Close();
                return false;
            case 1:
                _ipAddr = possiblesIp[0];
                LblIp.Text = "IP: " + _ipAddr;
                return true;
            default:
                _chooseIpWindow = new ChooseIp(possiblesIp);
                _chooseIpWindow.Show();
                _chooseIpWindow.Closing += (_,_) => ChooseIpWindowClosing();
                Hide();
                IsEnabled = false;
                return false;
        }
    }

    #endregion

    #region New Student

    /// <summary>
    /// Function that connect the students.
    /// </summary>
    private void LogClients()
    {
        IPEndPoint localEndPoint = new(_ipAddr, 11111);
        Socket listener = new(_ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(-1);
        while (_running)
        {
            try
            {
                Socket clientSocket = listener.Accept();
                Dispatcher.UIThread.Post(() => LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Nouvelle connexion de: " + clientSocket.RemoteEndPoint));
                _allStudents.Add(new DataForTeacher(clientSocket, _nextId));
                Dispatcher.UIThread.Post(() =>
                {
                    if (DataContext is MainViewModel model)
                    {
                        model.NewStudent(_nextId);
                    }
                });
                Task.Run(() => SendAuthorisedUrl(clientSocket));
                _nextId++;
            }
            catch (Exception e) { Dispatcher.UIThread.Post(() => LbxInfo.Items.Add(e.ToString())); }
        }
    }

    /// <summary>
    /// Function that sends the authorised urls to a student.
    /// </summary>
    /// <param name="socket"></param>
    private void SendAuthorisedUrl(Socket socket)
    {
        while (_isAsking) { Thread.Sleep(100); }
        _isAsking = true;
        socket.Send(new Command(CommandType.ReceiveAutorisedUrls).ToByteArray());
        //serialization
        string jsonString = JsonSerializer.Serialize(_properties.AutorisedWebsites);
        //sending
        Thread.Sleep(100);
        socket.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
        _isAsking = false;
    }

    #endregion

    #region Transfert

    /// <summary>
    /// Function that updates all students.
    /// </summary>
    private void AskingData()
    {
        while (_running)
        {
            if (_allStudents.Count != 0)
            {
                while (_isAsking) { Thread.Sleep(10); }
                _isAsking = true;
                DateTime startUpdate = DateTime.Now;
                DateTime nextUpdate = DateTime.Now.AddSeconds(_properties.TimeBetweenDemand);
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
                _isAsking = false;
                Dispatcher.UIThread.Post(() => LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Attente du prochain cycle dans " + (cycleDuration - updateDuration) + " secondes"));
                Thread.Sleep(cycleDuration - updateDuration);
            }
            else { Thread.Sleep(100); }
        }
    }

    /// <summary>
    /// Function that asks a student for their data and screenshots.
    /// </summary>
    /// <param name="clientToRemove"></param>
    private void UpdateStudents(ICollection<DataForTeacher> clientToRemove)
    {
        for (int i = 0; i < _allStudents.Count; i++)
        {
            Socket socket = _allStudents[i].SocketToStudent;
            socket.ReceiveTimeout = _properties.DefaultTimeout;
            socket.SendTimeout = _properties.DefaultTimeout;
            try
            {
                socket.Send(new Command(CommandType.DemandData).ToByteArray());
                LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande des données à " + _allStudents[i].UserName);
                int i1 = i;
                Task.Run(() => _allStudents[i1] = ReceiveData(_allStudents[i1])).Wait();
                socket.Send(new Command(CommandType.DemandImage).ToByteArray());
                LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande de l'image à " + _allStudents[i].UserName);
                int i2 = i;
                Task.Run(() => ReceiveImage(_allStudents[i2])).Wait();
            }
            catch (SocketException)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                clientToRemove.Add(_allStudents[i]);
            }
        }
    }

    /// <summary>
    /// Function that is used to receive the student data.
    /// </summary>
    /// <param name="student">The student that sent the data.</param>
    /// <returns></returns>
    private DataForTeacher ReceiveData(DataForTeacher student)
    {
        try
        {
            Socket socket = student.SocketToStudent;
            int id = student.Id;
            byte[] dataBuffer = new byte[100000];
            socket.ReceiveTimeout = _properties.DefaultTimeout;
            int nbData = socket.Receive(dataBuffer);
            Array.Resize(ref dataBuffer, nbData);
            Data? data = JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer));
            if (data == null) { return student;}
            student = new DataForTeacher(data)
            {
                SocketToStudent = socket,
                Id = id
            };
            LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Données recue de " + student.UserName);
            Task.Run(() => UpdateTreeViews(student));
            student.NumberOfFailure = 0;
            return student;
        }
        catch
        {
            LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé de donnée");
            student.NumberOfFailure++;
            return student;
        }
    }

    /// <summary>
    /// Function that receives the screenshot sent by the student.
    /// </summary>
    /// <param name="student">The student that sent the image.</param>
    private void ReceiveImage(DataForTeacher student)
    {
        try
        {
            Socket socket = student.SocketToStudent;
            byte[] imageBuffer = new byte[10485760];
            socket.ReceiveTimeout = _properties.DefaultTimeout;
            int nbData = socket.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
            Array.Resize(ref imageBuffer, nbData);
            student.ScreenShot = new MagickImage(new MemoryStream(imageBuffer));
            LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Image recue de " + student.UserName);
            student.NumberOfFailure = 0;
            PreviewDisplay.AddOrUpdatePreview(student.Id, student.ComputerName, student.ScreenShot);
        }
        catch
        {
            LbxInfo.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé d'image");
            student.NumberOfFailure++;
        }
    }

    #endregion

    #region TreeView update

    /// <summary>
    /// Function that updates the tree-views.
    /// </summary>
    /// <param name="student">The student that is updated.</param>
    private void UpdateTreeViews(DataForTeacher student)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if(DataContext is not MainViewModel model){return;}
            model.UpdateProcesses(student.Id,student.Processes);
            model.UpdateBrowsers(student.Id, student.Urls);
        });
    }

    #endregion

    /// <summary>
    /// Function that apply the filters in the tree-view.
    /// </summary>
    /// <param name="student">The student to remove.</param>
    private void RemoveStudent(DataForTeacher student)
    {
        if(DataContext is not MainViewModel model) {return;}
        model.RemoveStudent(student.Id);
    }

    #region Previews

    /// <summary>
    /// Function that creates or remove the screenshots when a checkbox is clicked.
    /// </summary>
    private void ShowHidePreview()
    { 
    }

    #endregion

    #region Stream

    /// <summary>
    /// Function that take screenshots and share them in multicast.
    /// </summary>
    private void RecordAndStreamScreen()
    {
        foreach (DataForTeacher student in _studentToShareScreen) { student.SocketToStudent.Send(new Command(CommandType.ReceiveMulticast).ToByteArray()); }

        Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPAddress ip = IPAddress.Parse("232.1.2.3");
        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.Parse("232.1.2.3").GetAddressBytes());
        s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10);
        IPEndPoint ipEndPoint = new(ip, 45678);
        s.Connect(ipEndPoint);
        Thread.Sleep(1000);
        _multicastSender = new ReliableMulticastSender(s, _properties.ScreenToShareId);
    }

    /// <summary>
    /// Function starts or stops the stream.
    /// </summary>
    private void ShareScreen()
    {
        if (!_isSharing)
        {
            ChooseStreamOptions prompt = new(_allStudents);
            prompt.Show();
            if (_studentToShareScreen.Count == 0) { return; }
            _isSharing = true;
            SendStreamConfiguration();
            _screenSharer = Task.Run(RecordAndStreamScreen);
            BtnShare.Content = "Stop Sharing";
        }
        else
        {
            _isSharing = false;
            _multicastSender.Sending = false;
            foreach (DataForTeacher student in _studentToShareScreen)
            {
                student.SocketToStudent.Send(new Command(CommandType.StopReceiveMulticast).ToByteArray());
            }
            _studentToShareScreen = new List<DataForTeacher>();
            BtnShare.Content = "Share screen";
            _screenSharer.Wait();
            _screenSharer.Dispose();
        }
    }

    /// <summary>
    /// Function that send the stream configuration to all relevant students.
    /// </summary>
    private void SendStreamConfiguration()
    {
        byte[] bytes = Encoding.Default.GetBytes(JsonSerializer.Serialize(_properties.Options));
        foreach (DataForTeacher student in _studentToShareScreen)
        {
            student.SocketToStudent.Send(new Command(CommandType.ApplyMulticastSettings).ToByteArray());
        }
        Thread.Sleep(100);
        foreach (DataForTeacher student in _studentToShareScreen)
        {
            student.SocketToStudent.Send(bytes);
        }
    }

    #endregion

    #region Teacher actions

    /// <summary>
    /// Function that shows the tray-icon if the application is minimized.
    /// </summary>
    public void TeacherAppResized()
    {
        if(DataContext is not MainViewModel model){return;}
        switch (WindowState)
        {
            case WindowState.Minimized:
                model.TrayIconVisible = true;
                Hide();
                break;
            default:
                model.TrayIconVisible = false;
                break;
        }
    }

    /// <summary>
    /// Function that reopens the application when the tray-icon is clicked.
    /// </summary>
    public void TrayIconTeacherClick()
    {
        Show();
        WindowState = WindowState.Normal;
    }

    /// <summary>
    /// Function that signal to the student the closure of the teacher application.
    /// </summary>
    private void OnClosing()
    {
        _running = false;
        foreach (Socket studentSocket in _allStudents.Select(student => student.SocketToStudent))
        {
            try
            {
                studentSocket.Send(new Command(CommandType.StopReceiveMulticast).ToByteArray());
                studentSocket.Send(new Command(CommandType.DisconnectOfTeacher).ToByteArray());
            }
            catch {/*Student has already closed the application.*/ }
            studentSocket.Dispose();
        }
    }

    /// <summary>
    /// Function that verifies the node click before opening a new display.
    /// </summary>
    private void OpenPrivateDisplay()
    {
    }

    #endregion

    #region Individual display

    /// <summary>
    /// Function that updates all individual displays.
    /// </summary>
    private void UpdateAllIndividualDisplay()
    {
        foreach (DisplayStudent display in _allStudentsDisplay)
        {
            foreach (DataForTeacher student in _allStudents.Where(student => display.GetStudentId() == student.Id))
            {
                display.UpdateDisplay(student);
            }
        }
    }

    /// <summary>
    /// Function that creates a new individual display.
    /// </summary>
    /// <param name="student">The student for which you want a private display.</param>
    private void OpenPrivateDisplay(DataForTeacher student)
    {
        if (_allStudentsDisplay.Any(display => display.GetStudentId() == student.Id))
        {
            return;
        }
        DisplayStudent newDisplay = new(student.Id);
        _allStudentsDisplay.Add(newDisplay);
        newDisplay.UpdateDisplay(student);
        newDisplay.Closing += (sender,_) =>RemovePrivateDisplay(sender);
        newDisplay.Show();
    }

    /// <summary>
    /// Function that removes the individual display when it is closed.
    /// </summary>
    /// <param name="sender"></param>
    private void RemovePrivateDisplay(object? sender)
    {
        if(sender == null){return;}
        DisplayStudent closingDisplay = (DisplayStudent)sender;
        _allStudentsDisplay.Remove(closingDisplay);
    }

    #endregion

    #region Filter

    /// <summary>
    /// Function that enable or disable the filters in the tree-views.
    /// </summary>
    private void ButtonFilter_Click()
    {
        _properties.FilterEnabled = !_properties.FilterEnabled;
        foreach (StudentNode? studentNode in TreeViewStudents.Items.Cast<StudentNode>())
        {
            if(studentNode == null){return;}

            studentNode.ApplyFilter(_properties.FilterEnabled);
        }
        BtnFilter.Content = _properties.FilterEnabled ? "Désactiver" : "Activer";
    }

    #endregion

    #region TreeView display

    /// <summary>
    /// Function that closes all tree-node in the tree-views.
    /// </summary>
    private void CloseAllTreeViewNodes()
    {
        
    }

    /// <summary>
    /// Function that opens all tree-node in the tree-views.
    /// </summary>
    private void OpenAllTreeViewNodes()
    {
        
    }

    #endregion

    /// <summary>
    /// Function that open the configuration window.
    /// </summary>
    private void OpenConfiguration()
    {
        Configuration configWindow = new();
        configWindow.Show();
    }
}