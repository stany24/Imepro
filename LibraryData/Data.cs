using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        public Historique Urls { get; set; }
        [JsonInclude]
        public Dictionary<int, string> Processes { get; set; }
        [JsonIgnore]
        public Bitmap ScreenShot { get; set; }

        #endregion

        #region Constructor

        public Data(string userName, string computerName, Historique urls, Dictionary<int, string> processes)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
        }

        public Data() {
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
        #region Variables/Constructeur

        readonly private List<string> DefaultProcess = new();
        readonly private ListBox lbxConnexion;
        readonly private PictureBox pbxScreenShot;
        readonly private ListBox tbxMessage;
        readonly private Form form;
        readonly private List<string> browsersList = new() { "chrome", "firefox", "iexplore", "safari", "opera", "msedge" };

        private int screenToStream;
        private readonly GlobalKeyboardHook gkh = new();
        private Rectangle OldRect = Rectangle.Empty;
        private StreamOptions options;
        private bool mouseDisabled = false;
        private bool isReceiving = false;
        private bool isControled = false;

        public Socket SocketToTeacher { get; set; }
        public IPAddress IpToTeacher { get; set; }
        public List<string> AutorisedUrls { get; set; }
        public List<int> SeleniumProcessesID { get; set; }
        Socket SocketMulticast;
        readonly List<byte> byteImage = new();

        public DataForStudent(ListBox lbxconnexion, PictureBox pbxscreenshot, ListBox tbxmessage, Form form)
        {
            lbxConnexion = lbxconnexion;
            pbxScreenShot = pbxscreenshot;
            tbxMessage = tbxmessage;
            this.form = form;
            AutorisedUrls = new();
            SeleniumProcessesID = new();
            GetDefaultProcesses();
            GetNames();
        }

        /// <summary>
        /// Function to get the computer name and the user name
        /// </summary>
        private void GetNames()
        {
            ComputerName = Environment.MachineName;
            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// Function returning an instance of the parent class
        /// </summary>
        /// <returns></returns>
        private Data ToData()
        {
            return new Data(UserName, ComputerName, Urls, Processes);
        }

        #endregion

        #region Récupération Url/Processus/Image

        /// <summary>
        /// Function to get the tab name in all browser.
        /// </summary>
        private void GetCurrentWebTabsName()
        {
            [DllImport("user32.dll")]
            static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            foreach (string singleBrowser in browsersList)
            {
                Process[] process = Process.GetProcessesByName(singleBrowser);
                if (process.Length > 0)
                {
                    foreach (Process instance in process)
                    {
                        Process parent = GetParent(instance);
                        if (parent != null && parent.ProcessName == singleBrowser)
                        { continue; }
                        Process.GetProcessById(instance.Id);
                        if (SeleniumProcessesID.Contains(instance.Id)) { continue; }
                        IntPtr hWnd = instance.MainWindowHandle;

                        StringBuilder text = new(GetWindowTextLength(hWnd) + 1);
                        _ = GetWindowText(hWnd, text, text.Capacity);
                        if (text.ToString() != "")
                        {
                            Urls.AddUrl(new Url(DateTime.Now, text.ToString()), singleBrowser);
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

        #region Connexion/Reception

        /// <summary>
        /// Function used to connect to the teacher application.
        /// </summary>
        public Socket ConnectToTeacher(int port)
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
                            Task.Run(WaitForDemand);
                            lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Connected"); }));
                            return sender;
                        }
                        else {
                            sender.Close();
                            lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Connexion failed to " +IpToTeacher + " Error: "+result); }));
                        }
                    }
                    // Manage of Socket's Exceptions
                    catch (ArgumentNullException ane)
                    {
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("ArgumentNullException : " + ane.ToString()); }));
                        Thread.Sleep(1000);
                    }
                    catch (SocketException se)
                    {
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("SocketException : " + se.ToString()); }));
                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Unexpected exception : " + e.ToString()); }));
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(e.ToString()); }));
                Thread.Sleep(1000);
            }
            return null;
        }

        /// <summary>
        /// Function waiting for teacher demand and responding correctly.
        /// </summary>
        private void WaitForDemand()
        {
            while (SocketToTeacher == null) { Thread.Sleep(100); }
            while (true)
            {
                byte[] info = new byte[12];
                int lenght;
                try { lenght = SocketToTeacher.Receive(info); }
                catch (SocketException) { return; }
                Array.Resize(ref info, lenght);
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(Encoding.Default.GetString(info)); }));
                string text = Encoding.Default.GetString(info);
                switch (Encoding.Default.GetString(info).Split(' ')[0])
                {
                    case "data":SendData(); break;
                    case "image":SendImage(TakeAllScreenShot(), SocketToTeacher); break;
                    case "kill": KillSelectedProcess(Convert.ToInt32(text.Split(' ')[1])); break;
                    case "receive": Task.Run(ReceiveMulticastStream); break;
                    case "apply": ApplyMulticastSettings(); break;
                    case "stops": Stop(); break;
                    case "message": ReceiveMessage(); break;
                    case "url": ReceiveAuthorisedUrls(); break;
                    case "control": screenToStream = Convert.ToInt32(text.Split(' ')[1]); Task.Run(() => SendStream()); break;
                    case "stopc": isControled = false; break;
                    case "mouse": break;
                    case "key": break;
                    case "disconnect": Disconnect(); return;
                    case "shutdown": ShutDown(); return;
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
            lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Le professeur a coupé la connexion"); }));
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
            form.Invoke(new MethodInvoker(delegate { form.FormBorderStyle = FormBorderStyle.Sizable; }));
            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Visible = false; }));
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
            tbxMessage.Invoke(new MethodInvoker(delegate { tbxMessage.Items.Add(DateTime.Now.ToString("hh:mm ") + Encoding.Default.GetString(bytemessage)); }));
        }

        #endregion

        #region Stream

        /// <summary>
        /// Function that receive the multicast stream form the teacher.
        /// </summary>
        private void ReceiveMulticastStream()
        {
            Task.Run(MinimizeUnAutorisedEverySecond);
            SocketMulticast = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 45678);
            SocketMulticast.Bind(ipep);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            SocketMulticast.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            SocketAsyncEventArgs receiveArgs = new();
            receiveArgs.SetBuffer(new byte[65000], 0, 65000);
            receiveArgs.Completed += ReceiveCompleted;
            BeginReceive(receiveArgs);
        }

        /// <summary>
        /// Function to start receiving the multicast stream.
        /// </summary>
        /// <param name="args"></param>
        private void BeginReceive(SocketAsyncEventArgs args)
        {
            isReceiving = true;
            bool willRaiseEvent = SocketMulticast.ReceiveAsync(args);
            if (!willRaiseEvent)
                ProcessReceive(args);
        }

        /// <summary>
        /// Function to end the multicast stream.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessReceive(args);
        }

        /// <summary>
        /// Function to receive the multicast stream.
        /// </summary>
        /// <param name="args"></param>
        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {

                int size = args.BytesTransferred;
                byte[] receivedData = new byte[size];
                Array.Copy(args.Buffer, args.Offset, receivedData, 0, size);
                byteImage.AddRange(receivedData);
                if(size != 65000){
                    Bitmap bitmap = new(new MemoryStream(byteImage.ToArray()));
                    pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = bitmap; }));
                    byteImage.Clear();
                }

                // Continue receiving if needed
                BeginReceive(args);
            }
            else
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Erreur de reception"); }));
                isReceiving = false;
            }
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
            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Visible = true; }));
            form.Invoke(new MethodInvoker(delegate
            {
                form.Show();
                form.Controls.SetChildIndex(pbxScreenShot, 0);
                switch (options.GetPriority())
                {
                    case Priority.Fullscreen:
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.WindowState = FormWindowState.Maximized;
                        break;
                    case Priority.Blocking:
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.WindowState = FormWindowState.Maximized;
                        form.TopMost = true;
                        mouseDisabled = true;
                        Task.Run(DisableMouseEverySecond);
                        DisableKeyboard();
                        break;
                    case Priority.Topmost:
                        form.TopMost = true;
                        form.FormBorderStyle = FormBorderStyle.None;
                        form.WindowState = FormWindowState.Maximized;
                        break;
                    case Priority.Widowed:
                        form.Controls.SetChildIndex(pbxScreenShot, 0);
                        break;
                }
            }));
        }

        #endregion

        #region Blocage

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
}
