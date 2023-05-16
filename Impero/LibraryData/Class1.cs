using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryData
{
    [Serializable]
    public class Data
    {
        [JsonInclude]
        public string UserName = "";
        [JsonInclude]
        public string ComputerName = "";
        [JsonInclude]
        public HistoriqueUrls Urls = new();
        [JsonInclude]
        public Dictionary<int, string> Processes = new();
        [JsonIgnore]
        public Bitmap ScreenShot;

        public Data(string userName, string computerName, HistoriqueUrls urls, Dictionary<int, string> processes)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
        }

        public Data() { }
    }

    [Serializable]
    public class StreamOptions
    {
        [JsonInclude]
        public Priority priority;
        [JsonInclude]
        public Focus focus;
        [JsonInclude]
        public List<string> AutorisedOpenedProcess;
        public StreamOptions(Priority priority, Focus focus, List<string> autorisedOpenedProcess)
        {
            this.priority = priority;
            this.focus = focus;
            AutorisedOpenedProcess = autorisedOpenedProcess;
        }
    }

    public enum Priority
    {
        Widowed,
        Fullscreen,
        Topmost,
        Blocking
    }

    public enum Focus
    {
        Everything,
        OneNote,
        VisualStudio,
        VSCode,
        Word,
    }

    public class DataForTeacher : Data
    {
        public Socket SocketToStudent;
        public Socket SocketControl = null;
        public int ID;
        public int NumberOfFailure;

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

        public override string ToString()
        {
            return UserName + " " + ComputerName;
        }
    }

    public class DataForStudent : Data, IMessageFilter
    {
        #region Variables/Constructeur
        private int screenToStream;
        private readonly GlobalKeyboardHook gkh = new();
        private Rectangle OldRect = Rectangle.Empty;
        private StreamOptions options;
        private bool mouseDisabled = false;
        private bool isReceiving = false;
        private bool isControled = false;
        public Socket SocketToTeacher;
        private Socket SocketControl;
        readonly private List<string> DefaultProcess = new();
        readonly private ListBox lbxConnexion;
        readonly private PictureBox pbxScreenShot;
        readonly private IPAddress IpToTeacher;
        readonly private ListBox tbxMessage;
        readonly private Form form;
        public List<string> AutorisedUrls = new();
        readonly private List<string> browsersList = new() { "chrome", "firefox", "iexplore", "safari", "opera", "msedge" };
        public DataForStudent(ListBox lbxconnexion, PictureBox pbxscreenshot, ListBox tbxmessage, IPAddress ipToTeacher, Form form)
        {
            lbxConnexion = lbxconnexion;
            pbxScreenShot = pbxscreenshot;
            tbxMessage = tbxmessage;
            IpToTeacher = ipToTeacher;
            this.form = form;
            GetDefaultProcesses();
            GetNames();
            Task.Run(GetAllTabNameEvery5Seconds);
        }

        /// <summary>
        /// Fonction qui permet de trouver le nom d'utilisateur et le nom de la machine
        /// </summary>
        private void GetNames()
        {
            ComputerName = Environment.MachineName;
            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// Fonction qui retourne un instance de la classe parente
        /// </summary>
        /// <returns></returns>
        private Data ToData()
        {
            return new Data(UserName, ComputerName, Urls, Processes);
        }

        #endregion

        #region Récupération Url/Processus/Image

        private void GetAllTabNameEvery5Seconds()
        {
            while (true)
            {
                GetCurrentWebTabsName();
                Thread.Sleep(5000);
            }
        }

        /// <summary>
        /// Fonction qui permet de récuperer le nom de l'onglet actif dans tous les navigateurs ouverts
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
                        IntPtr hWnd = instance.MainWindowHandle;

                        StringBuilder text = new(GetWindowTextLength(hWnd) + 1);
                        _ = GetWindowText(hWnd, text, text.Capacity);
                        if (text.ToString() != "")
                        {
                            Urls.AddUrl(new Url(DateTime.Now, singleBrowser, text.ToString()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fonction qui crée une list de processus lancé par l'ordinateur au démarrage de l'application
        /// </summary>
        private void GetDefaultProcesses()
        {
            foreach (Process process in Process.GetProcesses().OrderBy(x => x.ProcessName)) { DefaultProcess.Add(process.ProcessName); }
        }

        /// <summary>
        /// Fonction qui met à jour la list des processus lancé par l'utilisateur
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
        /// Fonction qui permet de prendre une capture d'écran de tous les écran puis de les recomposer en une seul image
        /// </summary>
        private Bitmap TakeAllScreenShot()
        {
            int TotalWidth = 0;
            int MaxHeight = 0;
            List<Bitmap> images = new();
            //prend une capture d'écran de tout les écran
            foreach (Screen screen in Screen.AllScreens)
            {
                TakeSreenShot(screen);
                images.Add(TakeSreenShot(screen));
                TotalWidth += screen.Bounds.Width;
                if (screen.Bounds.Height > MaxHeight) { MaxHeight = screen.Bounds.Height; }
            }
            if (MaxHeight > 0)
            {
                Bitmap FullImage = new(TotalWidth, MaxHeight, PixelFormat.Format16bppRgb565);
                Graphics FullGraphics = Graphics.FromImage(FullImage);

                int offsetLeft = 0;
                //Crée une seul image de toutes les captures d'écran
                foreach (Bitmap image in images)
                {
                    FullGraphics.DrawImage(image, new Point(offsetLeft, 0));
                    offsetLeft += image.Width;
                }
                //FullImage = (new Bitmap(FullImage, new Size(200,200)));
                FullGraphics.Dispose();
                return FullImage;
            }
            return null;
        }

        private Bitmap TakeSreenShot(Screen screen)
        {
            Bitmap Bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
            Rectangle ScreenSize = screen.Bounds;
            try
            {
                Graphics.FromImage(Bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
            }
            catch (Exception) { }
            return Bitmap;
        }

        #endregion

        #region Envoi de données

        /// <summary>
        /// Fonction qui sérialize les données puis les envoient au professeur
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
        /// Fonction qui envoie le screenshot au professeur
        /// </summary>
        private void SendImage(Bitmap image,Socket socket)
        {
            byte[] imagebytes;
            ImageConverter converter = new();
            imagebytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            socket.Send(imagebytes, 0, imagebytes.Length, SocketFlags.None);
        }

        #endregion

        #region Connexion/Reception

        /// <summary>
        /// Fonction qui connecte cette application à l'application du professeur
        /// </summary>
        public Socket ConnectToTeacher(int port)
        {
            try
            {
                // Establish the remote endpoint for the socket. This example uses port 11111 on the local computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint localEndPoint = new(IpToTeacher, port);

                // Creation TCP/IP Socket using Socket Class Constructor
                Socket sender = new(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                while (true)
                {
                    // Si l'addresse du professeur a changé on adapte le socket
                    if (localEndPoint.Address != IpToTeacher)
                    {
                        localEndPoint.Address = IpToTeacher;
                        sender = new(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    }
                    try
                    {
                        // Connect Socket to the remote endpoint using method Connect()
                        sender.Connect(localEndPoint);
                        Task.Run(WaitForDemand);
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Connected"); }));
                        return sender;
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
        /// Fonction qui attend les demandes du professeur et lance la bonne fonction pour y répondre
        /// </summary>
        private void WaitForDemand()
        {
            while(SocketToTeacher == null) {Thread.Sleep(100);}
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
                    case "data": SendData(); break;
                    case "image": ; SendImage(TakeAllScreenShot(),SocketToTeacher); break;
                    //case "kill": KillSelectedProcess(Convert.ToInt32(text.Split(' ')[1])); break;
                    case "receive":isReceiving = true;Task.Run(ReceiveMulticastStream); break;
                    case "apply": ApplyMulticastSettings(); break;
                    case "stops":Stop();break;
                    case "message": ReceiveMessage(); break;
                    case "url": ReceiveAuthorisedUrls(); break;
                    case "control":screenToStream = Convert.ToInt32(text.Split(' ')[1]);  Task.Run(()=>SendStream());break;
                    case "stopc":isControled = false; break;
                    case "mouse": break;
                    case "key": break;
                    case "disconnect":Disconnect();return;
                    case "shutdown":ShutDown();return;
                }
            }
        }

        private void Disconnect()
        {
            SocketToTeacher.Disconnect(false);
            SocketToTeacher = null;
            lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Le professeur a coupé la connexion"); }));
            Thread.Sleep(1000);
            SocketToTeacher = Task.Run(() => ConnectToTeacher(11111)).Result;
        }

        private void ShutDown()
        {
            SocketToTeacher.Disconnect(false);
            SocketToTeacher = null;
            Application.Exit();
        }

        private void Stop()
        {
            isReceiving = false;
            mouseDisabled = false;
            form.Invoke(new MethodInvoker(delegate { form.FormBorderStyle = FormBorderStyle.Sizable; }));
            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Visible = false; }));
            gkh.Unhook();
        }

        private void SendStream()
        {
            isControled = true;
            SocketControl = ConnectToTeacher(11112);
            while(isControled)
            {
                SendImage(TakeSreenShot(Screen.AllScreens[screenToStream]),SocketControl);
            }
        }

        /// <summary>
        /// Fonction qui recoit la liste des urls autorisés
        /// </summary>
        private void ReceiveAuthorisedUrls()
        {
            byte[] bytemessage = new byte[102400];
            int nbData = SocketToTeacher.Receive(bytemessage);
            Array.Resize(ref bytemessage, nbData);
            AutorisedUrls = JsonSerializer.Deserialize<List<string>>(Encoding.Default.GetString(bytemessage));
        }

        /// <summary>
        /// Fonction qui recoit un message du professeur
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
        /// Fonction qui recoit la diffusion multicast envoyée par le professeur
        /// </summary>
        private void ReceiveMulticastStream()
        {
            switch (options.focus)
            {
                case Focus.Everything:
                    break;
                default:
                    Task.Run(MinimizeUnAutorisedEverySecond);
                    break;
            }
            Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 45678);
            s.Bind(ipep);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
            while (isReceiving)
            {
                byte[] imageArray = new byte[9999999];
                int size = 0;
                int lastId = 0;
                do
                {
                    try
                    {
                        byte[] message = new byte[65000];
                        size = s.Receive(message);
                        Array.Resize(ref message, size);
                        Array.Copy(message, 0, imageArray, lastId, size);
                        lastId += size;
                    }
                    catch { }
                } while (size == 65000);
                Array.Resize(ref imageArray, lastId);
                try
                {
                    Bitmap bitmap = new(new MemoryStream(imageArray));
                    pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = bitmap; }));
                }
                catch { }
            }
        }

        private void ApplyMulticastSettings()
        {
            byte[] message = new byte[128];
            int size = SocketToTeacher.Receive(message);
            Array.Resize(ref message, size);
            options = JsonSerializer.Deserialize<StreamOptions>(Encoding.Default.GetString(message));
            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Visible = true; }));
            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Dock = DockStyle.Fill; }));
            form.Invoke(new MethodInvoker(delegate
            {
                form.Show();
                form.Controls.SetChildIndex(pbxScreenShot, 0);
                switch (options.priority)
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

        private void DisableKeyboard()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gkh.HookedKeys.Add(key);
            }
            gkh.KeyDown += new KeyEventHandler(HandleKeyPress);
            gkh.KeyUp += new KeyEventHandler(HandleKeyPress);
        }


        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void MinimizeUnAutorisedEverySecond()
        {
            while (isReceiving)
            {
                WindowMinimize.MinimizeUnAuthorised(options.AutorisedOpenedProcess);
                Thread.Sleep(3000);
            }
            WindowMinimize.ShowBack();
        }

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
        private void DisableMouse()
        {
            // Arbitrary location.
            Cursor.Clip = new Rectangle(0, 60, 1, 1);
            Cursor.Hide();
            Application.AddMessageFilter(this);
            foreach (var process in Process.GetProcessesByName("Taskmgr"))
            {
                process.Kill();
            }
        }

        #endregion
    }

    [DataContract]
    public class ScreenShotPart
    {
        [DataMember]
        public byte[] Part;

        public ScreenShotPart(byte[] part)
        {
            Part = part;
        }
    }

    [Serializable]
    public class HistoriqueUrls
    {
        [JsonInclude]
        public List<Url> chrome = new();
        [JsonInclude]
        public List<Url> firefox = new();
        [JsonInclude]
        public List<Url> seleniumchrome = new();
        [JsonInclude]
        public List<Url> seleniumfirefox = new();
        [JsonInclude]
        public List<Url> opera = new();
        [JsonInclude]
        public List<Url> edge = new();
        [JsonInclude]
        public List<Url> safari = new();
        [JsonInclude]
        public List<Url> iexplorer = new();
        [JsonInclude]
        public List<Url> custom = new();

        public void AddUrl(Url url)
        {
            switch (url.Browser)
            {
                case "chrome":
                    VerifyUrl(chrome, url);
                    break;
                case "firefox":
                    VerifyUrl(firefox, url);
                    break;
                case "seleniumchrome":
                    VerifyUrl(seleniumchrome, url);
                    break;
                case "seleniumfirefox":
                    VerifyUrl(seleniumfirefox, url);
                    break;
                case "opera":
                    VerifyUrl(opera, url);
                    break;
                case "msedge":
                    VerifyUrl(edge, url);
                    break;
                case "safari":
                    VerifyUrl(safari, url);
                    break;
                case "iexplorer":
                    VerifyUrl(iexplorer, url);
                    break;
                case "custom":
                    VerifyUrl(custom, url);
                    break;
                default: break;
            }
        }
        private void VerifyUrl(List<Url> list, Url url)
        {
            if (list.Count == 0) { list.Add(url); return; }
            if (list.Last().Name != url.Name) { list.Add(url); return; }
        }
    }

    [Serializable]
    public class Url
    {
        [JsonInclude]
        readonly public DateTime CaptureTime;
        [JsonInclude]
        readonly public string Browser;
        [JsonInclude]
        readonly public string Name;

        public Url(DateTime capturetime, string browser, string name)
        {
            CaptureTime = capturetime;
            Browser = browser;
            Name = name;
        }
        public override string ToString()
        {
            return CaptureTime.ToString("HH:mm:ss") + " " + Name;
        }
    }

    [Serializable]
    public class IpForTheWeek
    {
        [JsonInclude]
        public string[] lundi = new string[2];
        [JsonInclude]
        public string[] mardi = new string[2];
        [JsonInclude]
        public string[] mercredi = new string[2];
        [JsonInclude]
        public string[] jeudi = new string[2];
        [JsonInclude]
        public string[] vendredi = new string[2];
        [JsonInclude]
        public string[] samedi = new string[2];
        [JsonInclude]
        public string[] dimanche = new string[2];

        public IpForTheWeek(IpForTheWeek copy)
        {
            lundi = copy.lundi;
            mardi = copy.mardi;
            mercredi = copy.mercredi;
            jeudi = copy.jeudi;
            vendredi = copy.vendredi;
            samedi = copy.samedi;
            dimanche = copy.dimanche;
        }

        /// <summary>
        /// Fonction qui enregistre l'ip donnée au bonne endroit, qui dépand du jour et de l'heure de l'action
        /// </summary>
        /// <param name="ip"></param>
        public void SetIp(string ip)
        {
            try { IPAddress.Parse(ip); }
            catch { return; }

            DayOfWeek day = DateTime.Now.DayOfWeek;
            int MatinOuAprèsMidi = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { MatinOuAprèsMidi = 1; }
            switch (day)
            {
                case DayOfWeek.Monday: lundi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Tuesday: mardi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Wednesday: mercredi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Thursday: jeudi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Friday: vendredi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Saturday: samedi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Sunday: dimanche[MatinOuAprèsMidi] = ip; break;
            }
        }

        /// <summary>
        /// Fonction qui retourne la bonne ip en fonction du jour et de l'heure de l'appel
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int MatinOuAprèsMidi = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { MatinOuAprèsMidi = 1; }
            return day switch
            {
                DayOfWeek.Monday => lundi[MatinOuAprèsMidi],
                DayOfWeek.Tuesday => mardi[MatinOuAprèsMidi],
                DayOfWeek.Wednesday => mercredi[MatinOuAprèsMidi],
                DayOfWeek.Thursday => jeudi[MatinOuAprèsMidi],
                DayOfWeek.Friday => vendredi[MatinOuAprèsMidi],
                DayOfWeek.Saturday => samedi[MatinOuAprèsMidi],
                DayOfWeek.Sunday => dimanche[MatinOuAprèsMidi],
                _ => null,
            };
        }

        public IpForTheWeek()
        {
            lundi[0] = "157.26.227.198";
            lundi[1] = "157.26.227.198";
            mardi[0] = "157.26.227.198";
            mardi[1] = "157.26.227.198";
            mercredi[0] = "157.26.227.198";
            mercredi[1] = "157.26.227.198";
            jeudi[0] = "157.26.227.198";
            jeudi[1] = "157.26.227.198";
            vendredi[0] = "157.26.227.198";
            vendredi[1] = "157.26.227.198";
            samedi[0] = "157.26.227.198";
            samedi[1] = "157.26.227.198";
            dimanche[0] = "157.26.227.198";
            dimanche[1] = "157.26.227.198";
        }
    }


    public class GlobalKeyboardHook
    {
        #region Constant, Structure, and Delegate Definitions

        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        #endregion

        #region Instance Variables

        public List<Keys> HookedKeys = new();
        private IntPtr hHook = IntPtr.Zero;
        private static KeyboardHookProc hookProc;

        #endregion

        #region Events

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        #endregion

        #region Constructors and Destructors

        public GlobalKeyboardHook()
        {
            hookProc = HookCallback;
            Hook();
        }

        ~GlobalKeyboardHook()
        {
            Unhook();
        }

        #endregion

        #region Public Methods

        public void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(hHook);
        }

        #endregion

        #region Private Methods

        private int HookCallback(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }
            return CallNextHookEx(hHook, code, wParam, ref lParam);
        }

        #endregion

        #region DLL Imports

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion
    }

    public class WindowMinimize
    {
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void MinimizeUnAuthorised(List<string> autorisedProcesses)
        {
            Process thisProcess = Process.GetCurrentProcess();
            List<Process> processes = Process.GetProcesses().ToList();

            foreach (Process process in processes)
            {
                if(process.ProcessName != autorisedProcesses[0] && process.ProcessName != thisProcess.ProcessName)
                {
                    try { ShowWindow(process.MainWindowHandle, SW_SHOWMINIMIZED); } catch { }
                }
            }
        }

        public static void ShowBack()
        {
            List<Process> processes = Process.GetProcesses().ToList();
            foreach (Process process in processes)
            {
                try { ShowWindow(process.MainWindowHandle, SW_SHOW); } catch { }
            }
        }
    }
}
