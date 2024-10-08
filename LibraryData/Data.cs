﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryData
{
    /// <summary>
    /// Class  that represent a basic student.
    /// </summary>
    [Serializable]
    public class Data
    {
        #region Variables

        [JsonInclude]
        public string UserName { get; set; }
        [JsonInclude]
        public string ComputerName { get; set; }
        [JsonInclude]
        public History Urls { get; set; }
        [JsonInclude]
        public Dictionary<int, string> Processes { get; set; }
        [JsonIgnore]
        public Bitmap ScreenShot { get; set; }

        #endregion

        #region Constructor

        public Data(string userName, string computerName, History urls, Dictionary<int, string> processes)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
        }

        public Data()
        {
            Urls = new();
            Processes = new();
        }

        #endregion
    }

    /// <summary>
    /// Class that represent a student in the teacher application.
    /// </summary>
    public class DataForTeacher : Data
    {
        #region Variables

        public Socket SocketToStudent { get; set; }
        public Socket SocketControl { get; set; }
        public int ID { get; set; }
        public int NumberOfFailure { get; set; }

        #endregion

        #region Constructor

        public DataForTeacher(Socket socket, int id)
        {
            SocketToStudent = socket;
            ID = id;
        }

        public DataForTeacher(Data data)
        {
            UserName = data.UserName;
            ComputerName = data.ComputerName;
            Urls = data.Urls;
            Processes = data.Processes;
        }

        #endregion
    }

    /// <summary>
    /// Class containing all logic for the student application.
    /// </summary>
    public class DataForStudent : Data, IMessageFilter
    {
        #region Variables/Events

        readonly private List<string> DefaultProcess = new();
        readonly private GlobalKeyboardHook gkh = new();
        readonly private Dictionary<string, BrowserName> browsersList = new() {
            { "chrome",BrowserName.Chrome },
            { "firefox", BrowserName.Firefox },
            { "iexplore",BrowserName.IExplorer },
            { "safari",BrowserName.Safari},
            { "opera", BrowserName.Opera },
            { "msedge",BrowserName.Edge } };

        private ReliableMulticastReceiver MulticastReceiver { get; set; }
        private Rectangle OldRect = Rectangle.Empty;
        private StreamOptions options;
        private int screenToStream;

        private bool mouseDisabled = false;
        private bool isReceiving = false;
        private bool isControled = false;

        public event EventHandler<NewMessageEventArgs> NewConnexionMessageEvent;
        public event EventHandler<ChangePropertyEventArgs> ChangePropertyEvent;
        public event EventHandler<NewMessageEventArgs> NewMessageEvent;
        public event EventHandler<NewImageEventArgs> NewImageEvent;

        public Socket SocketToTeacher { get; set; }
        public IPAddress IpToTeacher { get; set; }
        public List<string> AutorisedUrls { get; set; }
        public List<int> SeleniumProcessesID { get; set; }

        #endregion

        #region Constructor

        public DataForStudent(IPAddress teacherIp)
        {
            IpToTeacher = teacherIp;
            AutorisedUrls = new();
            SeleniumProcessesID = new();
            GetDefaultProcesses();
            ComputerName = Environment.MachineName;
            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Task.Run(InitializeSocket);
        }

        private void InitializeSocket()
        {
            SocketToTeacher = ConnectToTeacher(11111);
            WaitForDemand();
        }

        #endregion

        #region Retrival of Url/Processes/Image

        /// <summary>
        /// Function to get the tab name in all browser.
        /// </summary>
        private void GetCurrentWebTabsName()
        {
            [DllImport("user32.dll")]
            static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            foreach (KeyValuePair<string, BrowserName> singleBrowser in browsersList)
            {
                Process[] process = Process.GetProcessesByName(singleBrowser.Key);
                if (process.Length > 0)
                {
                    foreach (Process instance in process)
                    {
                        Process parent = GetParent(instance);
                        if (parent != null && parent.ProcessName == singleBrowser.Key)
                        { continue; }
                        Process.GetProcessById(instance.Id);
                        if (SeleniumProcessesID.Contains(instance.Id)) { continue; }
                        IntPtr hWnd = instance.MainWindowHandle;

                        StringBuilder text = new(GetWindowTextLength(hWnd) + 1);
                        _ = GetWindowText(hWnd, text, text.Capacity);
                        if (text.ToString() != "")
                        {
                            Urls.AddUrl(new Url(DateTime.Now, text.ToString()), singleBrowser.Value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Function to get the parent of a process.
        /// </summary>
        /// <param name="process">The process you want to parent.</param>
        /// <returns>The parent of the given process.</returns>
        public Process GetParent(Process process)
        {
            try
            {
                using var query = new ManagementObjectSearcher(
                  "SELECT * " +
                  "FROM Win32_Process " +
                  "WHERE ProcessId=" + process.Id);
                return query
                  .Get()
                  .OfType<ManagementObject>()
                  .Select(p => Process.GetProcessById((int)(uint)p["ParentProcessId"]))
                  .FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Function to get the default processes when launching the application.
        /// </summary>
        private void GetDefaultProcesses()
        {
            foreach (Process process in Process.GetProcesses().OrderBy(x => x.ProcessName)) { DefaultProcess.Add(process.ProcessName); }
        }

        /// <summary>
        /// Function to update the processes launched by the user.
        /// </summary>
        private void GetUserProcesses()
        {
            Processes.Clear();
            List<Process> list = Process.GetProcesses().OrderBy(x => x.ProcessName).ToList();
            foreach (Process process in list)
            {
                if (!DefaultProcess.Contains(process.ProcessName)) { Processes.Add(process.Id, process.ProcessName); }
            }
        }

        /// <summary>
        /// Function to get a screenshot of all screen in one image.
        /// </summary>
        private Bitmap TakeAllScreenShot()
        {
            int TotalWidth = 0;
            int MaxHeight = 0;
            List<Bitmap> images = new();
            foreach (Screen screen in Screen.AllScreens)
            {
                images.Add(TakeSreenShot(screen));
                TotalWidth += screen.Bounds.Width;
                if (screen.Bounds.Height > MaxHeight) { MaxHeight = screen.Bounds.Height; }
            }
            if (MaxHeight > 0)
            {
                Bitmap FullImage = new(TotalWidth, MaxHeight, PixelFormat.Format16bppRgb565);
                Graphics FullGraphics = Graphics.FromImage(FullImage);

                int offsetLeft = 0;
                foreach (Bitmap image in images)
                {
                    FullGraphics.DrawImage(image, new Point(offsetLeft, 0));
                    offsetLeft += image.Width;
                }
                FullGraphics.Dispose();
                return FullImage;
            }
            return null;
        }

        /// <summary>
        /// Function to take a screenshot of the given screen.
        /// </summary>
        /// <param name="screen">The screen we want the screenshot.</param>
        /// <returns></returns>
        private Bitmap TakeSreenShot(Screen screen)
        {
            Bitmap Bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
            Rectangle ScreenSize = screen.Bounds;
            Graphics.FromImage(Bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
            return Bitmap;
        }

        #endregion

        #region Envoi de données

        /// <summary>
        /// Function send the data to the teacher.
        /// </summary>
        private void SendData()
        {
            GetCurrentWebTabsName();
            GetUserProcesses();
            //serialization
            string jsonString = JsonSerializer.Serialize(ToData(), new JsonSerializerOptions { IncludeFields = true, });
            //envoi
            SocketToTeacher.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
        }

        /// <summary>
        /// Function returning an instance of the parent class
        /// </summary>
        /// <returns></returns>
        private Data ToData()
        {
            return new Data(UserName, ComputerName, Urls, Processes);
        }

        /// <summary>
        /// Function to send the screenshot to the teacher.
        /// </summary>
        private void SendImage(Bitmap image, Socket socket)
        {
            byte[] imagebytes;
            ImageConverter converter = new();
            imagebytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            socket.Send(imagebytes, 0, imagebytes.Length, SocketFlags.None);
        }

        #endregion

        #region Connexion

        /// <summary>
        /// Function used to connect to the teacher application.
        /// </summary>
        private Socket ConnectToTeacher(int port)
        {
            try
            {
                int timeout = 2000;
                // Establish the remote endpoint for the socket. This example uses port 11111 on the local computer.
                IPEndPoint localEndPoint = new(IpToTeacher, port);


                while (true)
                {
                    // Creation TCP/IP Socket using Socket Class Constructor
                    Socket sender = new(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // Si l'addresse du professeur a changé on adapte le socket
                    if (localEndPoint.Address != IpToTeacher)
                    {
                        localEndPoint.Address = IpToTeacher;
                        sender = new(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    }
                    try
                    {
                        // Connect Socket to the remote endpoint using method Connect()
                        var result = sender.BeginConnect(localEndPoint, null, null);

                        bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
                        if (success)
                        {
                            sender.EndConnect(result);
                            NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs("Connected"));
                            return sender;
                        }
                        else
                        {
                            sender.Close();
                            NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs("Connexion failed to " + IpToTeacher + " Error: " + result));
                        }
                    }
                    // Manage of Socket's Exceptions
                    catch (ArgumentNullException ane)
                    {
                        NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs("ArgumentNullException : " + ane.ToString()));
                        Thread.Sleep(1000);
                    }
                    catch (SocketException se)
                    {
                        NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs("SocketException : " + se.ToString()));
                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs("Unexpected exception : " + e.ToString()));
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs(e.ToString()));
                Thread.Sleep(1000);
            }
            return null;
        }

        #endregion

        #region Receving/Applying

        /// <summary>
        /// Function waiting for teacher demand and responding correctly.
        /// </summary>
        private void WaitForDemand()
        {
            while (SocketToTeacher == null) { Thread.Sleep(100); }
            while (true)
            {
                byte[] info = new byte[128];
                int lenght;
                try { lenght = SocketToTeacher.Receive(info); }
                catch (SocketException) { return; }
                Array.Resize(ref info, lenght);
                Command command = JsonSerializer.Deserialize<Command>(Encoding.Default.GetString(info));
                NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs(command.ToString()));
                switch (command.Type)
                {
                    case CommandType.DemandData: SendData(); break;
                    case CommandType.DemandImage: SendImage(TakeAllScreenShot(), SocketToTeacher); break;
                    case CommandType.KillProcess: KillSelectedProcess(Convert.ToInt32(command.Args[1])); break;
                    case CommandType.ReceiveMulticast: Task.Run(ReceiveMulticastStream); break;
                    case CommandType.ApplyMulticastSettings: ApplyMulticastSettings(); break;
                    case CommandType.StopReceiveMulticast: Stop(); break;
                    case CommandType.ReceiveMessage: ReceiveMessage(); break;
                    case CommandType.ReceiveAutorisedUrls: ReceiveAuthorisedUrls(); break;
                    case CommandType.GiveControl: screenToStream = Convert.ToInt32(command.Args[1]); Task.Run(() => SendStream()); break;
                    case CommandType.StopControl: isControled = false; break;
                    case CommandType.DisconnectOfTeacher: Disconnect(); return;
                    case CommandType.StopApplication: ShutDown(); return;
                }
            }
        }

        /// <summary>
        /// Function to stop a process.
        /// </summary>
        /// <param name="id">the id of the process</param>
        private void KillSelectedProcess(int id)
        {
            Process.GetProcessById(id).Kill();
        }

        /// <summary>
        /// Function to reconnect to the teacher.
        /// </summary>
        private void Disconnect()
        {
            SocketToTeacher.Disconnect(false);
            SocketToTeacher = null;
            NewConnexionMessageEvent.Invoke(this, new NewMessageEventArgs("Le professeur a coupé la connexion"));
            Thread.Sleep(1000);
            SocketToTeacher = Task.Run(() => ConnectToTeacher(11111)).Result;
        }

        /// <summary>
        /// Function to stop the application.
        /// </summary>
        private void ShutDown()
        {
            SocketToTeacher.Disconnect(false);
            SocketToTeacher = null;
            Application.Exit();
        }

        /// <summary>
        /// Function to go back to normal after a stream.
        /// </summary>
        private void Stop()
        {
            isReceiving = false;
            mouseDisabled = false;
            ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form",ControlType.Window, "FormBorderStyle", FormBorderStyle.Sizable));
            ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("pbxScreenShot", ControlType.Image, "Visible", false));
            gkh.Unhook();
        }

        /// <summary>
        /// Function to send the stream to the teacher.
        /// </summary>
        private void SendStream()
        {
            isControled = true;
            Socket SocketControl = ConnectToTeacher(11112);
            while (isControled)
            {
                SendImage(TakeSreenShot(Screen.AllScreens[screenToStream]), SocketControl);
            }
        }

        /// <summary>
        /// Function to receive the autorised urls.
        /// </summary>
        private void ReceiveAuthorisedUrls()
        {
            byte[] bytemessage = new byte[102400];
            int nbData = SocketToTeacher.Receive(bytemessage);
            Array.Resize(ref bytemessage, nbData);
            AutorisedUrls = JsonSerializer.Deserialize<List<string>>(Encoding.Default.GetString(bytemessage));
        }

        /// <summary>
        /// Function to receive a message from the teacher.
        /// </summary>
        private void ReceiveMessage()
        {
            byte[] bytemessage = new byte[1024];
            int nbData = SocketToTeacher.Receive(bytemessage);
            Array.Resize(ref bytemessage, nbData);
            NewMessageEvent.Invoke(this, new NewMessageEventArgs(DateTime.Now.ToString("hh:mm ") + Encoding.Default.GetString(bytemessage)));
        }

        #endregion

        #region Stream

        /// <summary>
        /// Function that receive the multicast stream form the teacher.
        /// </summary>
        private void ReceiveMulticastStream()
        {
            Task.Run(MinimizeUnAutorisedEverySecond);
            Socket SocketMulticast = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 45678);
            SocketMulticast.Bind(ipep);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            SocketMulticast.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            MulticastReceiver = new ReliableMulticastReceiver(SocketMulticast);
            MulticastReceiver.NewImageEvent += DisplayImage;
        }

        /// <summary>
        /// Function that changes the displayed image by the new one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DisplayImage(object sender, NewImageEventArgs e)
        {
            NewImageEvent.Invoke(this, e);
        }

        /// <summary>
        /// Function that receive the multicast stream settings and apply them.
        /// </summary>
        private void ApplyMulticastSettings()
        {
            byte[] message = new byte[128];
            int size = SocketToTeacher.Receive(message);
            Array.Resize(ref message, size);
            options = JsonSerializer.Deserialize<StreamOptions>(Encoding.Default.GetString(message));
            ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("pbxScreenShot", ControlType.Image, "Visible", true));

            switch (options.GetPriority())
            {
                case Priority.Fullscreen:
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "FormBorderStyle", FormBorderStyle.None));
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "WindowState", FormWindowState.Maximized));
                    break;
                case Priority.Blocking:
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "FormBorderStyle", FormBorderStyle.None));
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "WindowState", FormWindowState.Maximized));
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "TopMost", true));
                    mouseDisabled = true;
                    Task.Run(DisableMouseEverySecond);
                    DisableKeyboard();
                    break;
                case Priority.Topmost:
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "FormBorderStyle", FormBorderStyle.None));
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "WindowState", FormWindowState.Maximized));
                    ChangePropertyEvent.Invoke(this, new ChangePropertyEventArgs("form", ControlType.Window, "TopMost", true));
                    break;
                case Priority.Widowed:
                    break;
            }
        }

        #endregion

        #region Blocking user

        /// <summary>
        /// Function to block all keyboard inputs.
        /// </summary>
        private void DisableKeyboard()
        {
            gkh.Hook();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gkh.HookedKeys.Add(key);
            }
            gkh.KeyDown += new KeyEventHandler(HandleKeyPress);
            gkh.KeyUp += new KeyEventHandler(HandleKeyPress);
        }

        /// <summary>
        /// Function to ignore pressed key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Function to minimized unauthorised application every second.
        /// </summary>
        private void MinimizeUnAutorisedEverySecond()
        {
            while (isReceiving)
            {
                WindowMinimize.MinimizeUnAuthorised(options.GetFocus());
                Thread.Sleep(3000);
            }
            WindowMinimize.ShowBack();
        }

        /// <summary>
        /// Function that disables the mouse every second.
        /// </summary>
        private void DisableMouseEverySecond()
        {
            OldRect = Cursor.Clip;
            while (mouseDisabled)
            {
                DisableMouse();
                Thread.Sleep(1000);
            }
            EnableMouse();
        }


        /// <summary>
        /// Function to block the mouse.
        /// </summary>
        private void DisableMouse()
        {
            Cursor.Clip = new Rectangle(0, 60, 1, 1);
            Cursor.Hide();
            Application.AddMessageFilter(this);
            foreach (var process in Process.GetProcessesByName("Taskmgr"))
            {
                process.Kill();
            }
        }

        /// <summary>
        /// Function to enable the mouse.
        /// </summary>
        private void EnableMouse()
        {
            Cursor.Clip = OldRect;
            Cursor.Show();
            Application.RemoveMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x201 || m.Msg == 0x202 || m.Msg == 0x203) return true;
            if (m.Msg == 0x204 || m.Msg == 0x205 || m.Msg == 0x206) return true;
            return false;
        }

        #endregion
    }

    /// <summary>
    /// Event to signal a new message
    /// </summary>
    public class NewMessageEventArgs : EventArgs
    {
        public string Message { get; }
        public NewMessageEventArgs(string message) { Message = message; }
    }

    /// <summary>
    /// Event to signal a change of property is needed
    /// </summary>
    public class ChangePropertyEventArgs : EventArgs
    {
        public string ControlName { get; }
        public ControlType ControlType { get; }
        public string PropertyName { get; }
        public object PropertyValue { get; }

        public ChangePropertyEventArgs(string controlName,ControlType controltype, string propertyName, object propertyValue)
        {
            ControlName = controlName;
            ControlType = controltype;
            PropertyName = propertyName;
            PropertyValue = propertyValue;
        }
    }

    public enum ControlType
    {
        Image,
        Window
    }
}
