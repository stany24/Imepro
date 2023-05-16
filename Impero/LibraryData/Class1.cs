using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
    /// Class parente, utilisé pour les données qui sont transferé de l'élève au professeur
    /// </summary>
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

    /// <summary>
    /// Classe qui contient les options du stream
    /// </summary>
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

    /// <summary>
    /// Classe qui représente un élève dans l'application professeur
    /// </summary>
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

    /// <summary>
    /// Classe qui contient toute la logique pour faire fonctionner l'application cliente
    /// </summary>
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

        /// <summary>
        /// Fonction qui récupére les urls toutes les secondes
        /// </summary>
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

        /// <summary>
        /// Fonction qui prend en screenshot l'écran demandé
        /// </summary>
        /// <param name="screen">L'écran que l'on veux prendre en screenshot</param>
        /// <returns></returns>
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
                    case "kill": KillSelectedProcess(Convert.ToInt32(text.Split(' ')[1])); break;
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

        /// <summary>
        /// Fonction qui arrête un processus
        /// </summary>
        /// <param name="id">l'id du processus que l'on veux arreter</param>
        private void KillSelectedProcess(int id)
        {
            Process.GetProcessById(id).Kill();
        }

        /// <summary>
        /// Fonction qui permet à l'élève de se reconnecter après une déconnection
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
        /// Fonction qui arrête le l'application à la demande du professeur
        /// </summary>
        private void ShutDown()
        {
            SocketToTeacher.Disconnect(false);
            SocketToTeacher = null;
            Application.Exit();
        }

        /// <summary>
        /// Fonction qui remet l'application dans un état normal après un stream
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
        /// Fonction qui envoie le stream au professeur
        /// </summary>
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

        /// <summary>
        /// Fonction qui recoit les paramêtre du stream et les appliques
        /// </summary>
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

        /// <summary>
        /// Fonction qui bloque toutes les touches du clavier
        /// </summary>
        private void DisableKeyboard()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                gkh.HookedKeys.Add(key);
            }
            gkh.KeyDown += new KeyEventHandler(HandleKeyPress);
            gkh.KeyUp += new KeyEventHandler(HandleKeyPress);
        }

        /// <summary>
        /// Fonction qui gére les touches pressé en les ignorants
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Fonction qui mininmize les applications non autorisées toutes les secondes
        /// </summary>
        private void MinimizeUnAutorisedEverySecond()
        {
            while (isReceiving)
            {
                WindowMinimize.MinimizeUnAuthorised(options.AutorisedOpenedProcess);
                Thread.Sleep(3000);
            }
            WindowMinimize.ShowBack();
        }

        /// <summary>
        /// Fonction qui désactive la souris toutes les seconde
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
        /// Fonction qui bloque la souris
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
        /// Fonction qui réactive la souris
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
    /// Classe qui reprèsente un historique des urls pour tous les navigateurs
    /// </summary>
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

        /// <summary>
        /// Fonction qui vérifie si l'url que l'on donne n'est pas déja le dernier de la list
        /// </summary>
        /// <param name="list">la list d'url</param>
        /// <param name="url">le nouvelle url</param>
        private void VerifyUrl(List<Url> list, Url url)
        {
            if (list.Count == 0) { list.Add(url); return; }
            if (list.Last().Name != url.Name) { list.Add(url); return; }
        }
    }

    /// <summary>
    /// Classe qui représente un Url
    /// </summary>
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

    /// <summary>
    /// Classe qui contient toutes les adresses ip pour une semaine
    /// </summary>
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

    /// <summary>
    /// Classe pour intercepter les touches du clavier avant quelles n'attaigne les applications
    /// </summary>
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

        /// <summary>
        /// Démmare l'interception des touches
        /// </summary>
        public void Hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        /// <summary>
        /// Arrête l'interception des touches
        /// </summary>
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

    /// <summary>
    /// Classe pour gérer l'affichage des autres applications
    /// </summary>
    public class WindowMinimize
    {
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOW = 5;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Fonction qui minimize toutes les applications interdites
        /// </summary>
        /// <param name="autorisedProcesses"></param>
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
        /// <summary>
        /// Fonction qui affiche à nouveau toutes les applications
        /// </summary>
        public static void ShowBack()
        {
            List<Process> processes = Process.GetProcesses().ToList();
            foreach (Process process in processes)
            {
                try { ShowWindow(process.MainWindowHandle, SW_SHOW); } catch { }
            }
        }
    }


    /// <summary>
    /// Classe pour les miniatures: une capture d'écran avec en desous le nom du poste
    /// </summary>
    public class Miniature : UserControl
    {
        public int StudentID;
        public PictureBox PbxImage = new();
        private readonly Label lblComputerInformations = new();
        private readonly Button btnSaveScreenShot = new();
        readonly int MargeBetweenText = 5;
        public int TimeSinceUpdate = 0;
        public string ComputerName;
        private readonly string SavePath;

        /// <summary>
        /// Consctucteur pour créer et positionner un miniature
        /// </summary>
        /// <param name="image">L'image à afficher</param>
        /// <param name="name">Le nom de l'ordinateur</param>
        /// <param name="studentID">L'id de l'élève</param>
        /// <param name="savepath">Le chemin de sauvegarde des images</param>
        public Miniature(Bitmap image, string name, int studentID, string savepath)
        {
            //valeurs pour la fenêtre de control
            Size = PbxImage.Size;
            StudentID = studentID;
            ComputerName = name;
            SavePath = savepath;

            PbxImage = new PictureBox
            {
                Location = new Point(0, 0),
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(400, 100),
            };
            PbxImage.SizeChanged += new EventHandler(UpdatePositionsRelativeToImage);
            PbxImage.LocationChanged += new EventHandler(UpdatePositionsRelativeToImage);
            Controls.Add(PbxImage);

            lblComputerInformations = new Label
            {
                Location = new Point(140, 0),
                Size = new Size(100, 20),
                Text = ComputerName + " " + TimeSinceUpdate,
            };
            Controls.Add(lblComputerInformations);

            btnSaveScreenShot = new Button
            {
                Location = new Point(0, 0),
                Size = new Size(80, 21),
                Text = "Sauvegarder"
            };
            btnSaveScreenShot.Click += new EventHandler(SaveScreenShot);
            Controls.Add(btnSaveScreenShot);
            UpdatePositionsRelativeToImage(new object(), new EventArgs());
        }

        /// <summary>
        /// Fonction pour sauvegarder la capture d'écran actuel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveScreenShot(object sender, EventArgs e)
        {
            PbxImage.Image.Save(SavePath + ComputerName + DateTime.Now.ToString("_yyyy-mm-dd_hh-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        /// <summary>
        /// Fonction qui ajoute une seconde au temps depuis la mise à jour de l'image et change le texte du label.
        /// </summary>
        public void UpdateTime()
        {
            TimeSinceUpdate++;
            try { lblComputerInformations.Invoke(new MethodInvoker(delegate { lblComputerInformations.Text = ComputerName + " " + TimeSinceUpdate + "s"; })); }
            catch { };
        }

        /// <summary>
        /// Fonction qui positionne le label par rapport à la picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePositionsRelativeToImage(object sender, EventArgs e)
        {
            //taille de la picturebox
            Size = new Size(PbxImage.Width, PbxImage.Height + 3 * MargeBetweenText + lblComputerInformations.Height);
            //postion du bouton
            btnSaveScreenShot.Left = PbxImage.Location.X + PbxImage.Width / 2 + MargeBetweenText / 2;
            btnSaveScreenShot.Top = PbxImage.Location.Y + PbxImage.Height + MargeBetweenText;
            //position du label
            lblComputerInformations.Left = PbxImage.Location.X + PbxImage.Width / 2 - lblComputerInformations.Width - MargeBetweenText / 2;
            lblComputerInformations.Top = btnSaveScreenShot.Location.Y + (btnSaveScreenShot.Height - lblComputerInformations.Height);
        }
    }

    /// <summary>
    /// Classe pour Afficher plusieurs miniatures dans un panel
    /// </summary>
    public class MiniatureDisplayer
    {
        public List<Miniature> MiniatureList = new();
        private int MaxWidth;
        private readonly int Marge = 10;
        public double zoom = 0.1;

        /// <summary>
        /// Fonction qui permet de zoomer dans les miniatures en changant leur taille
        /// </summary>
        public void ChangeZoom()
        {
            foreach (Miniature miniature in MiniatureList)
            {
                double NewHeight = miniature.PbxImage.Image.Height * zoom;
                double NewWidth = miniature.PbxImage.Image.Width * zoom;
                miniature.PbxImage.Height = Convert.ToInt32(NewHeight);
                miniature.PbxImage.Width = Convert.ToInt32(NewWidth);
            }
            UpdateAllLocations(MaxWidth);
        }

        public MiniatureDisplayer(int maxwidth)
        {
            MaxWidth = maxwidth;
            Task.Run(LaunchTimeUpdate);
        }

        /// <summary>
        /// Fonction qui toutes les seconde lance une mise à jour du temps
        /// </summary>
        private void LaunchTimeUpdate()
        {
            Thread.Sleep(3000);
            while (true)
            {
                Thread.Sleep(1000);
                Task.Run(UpdateAllTimes);
            }
        }

        /// <summary>
        /// Fonction qui lance la mise à jour du temps dans toutes les miniatures
        /// </summary>
        private void UpdateAllTimes()
        {
            foreach (Miniature miniature in MiniatureList)
            {
                miniature.UpdateTime();
            }
        }

        /// <summary>
        /// Fonction qui place toutes les miniatures au bon endroit
        /// </summary>
        public void UpdateAllLocations(int maxwidth)
        {
            MaxWidth = maxwidth;
            int OffsetTop = 0;
            int OffsetRight = 0;
            int MaxHeightInRow = 0;
            for (int i = 0; i < MiniatureList.Count; i++)
            {
                if (OffsetRight + MiniatureList[i].Width > MaxWidth)
                {
                    OffsetTop += MaxHeightInRow;
                    MaxHeightInRow = 0;
                    OffsetRight = 0;
                }
                MiniatureList[i].Top = OffsetTop;
                MiniatureList[i].Left = OffsetRight + Marge;
                OffsetRight += MiniatureList[i].Width + Marge;
                if (MiniatureList[i].Height > MaxHeightInRow) { MaxHeightInRow = MiniatureList[i].Height; }
            }
        }

        /// <summary>
        /// Fonction pour mettre à jour l'image d'une miniature
        /// </summary>
        /// <param name="id">Id de l'élève</param>
        /// <param name="computername"> Nom de l'ordinateur</param>
        /// <param name="image">La nouvelle image que l'on veux mettre</param>
        public void UpdateMiniature(int id, string computername, Bitmap image)
        {
            foreach (Miniature miniature in MiniatureList)
            {
                if (miniature.StudentID == id && miniature.ComputerName == computername)
                {
                    miniature.PbxImage.Image = image;
                    miniature.TimeSinceUpdate = 0;
                    return;
                }
            }
        }

        /// <summary>
        /// Fonction pour ajouter une miniature que le miniatureDisplayer doit gérer
        /// </summary>
        /// <param name="miniature"></param>
        public void AddMiniature(Miniature miniature)
        {
            MiniatureList.Add(miniature);
            ChangeZoom();
        }

        /// <summary>
        /// Fonction pour enlever un miniature de la liste que le miniatureDisplayer doit gérer
        /// </summary>
        /// <param name="id">Id de l'éléve</param>
        /// <param name="computername">Le nom de l'ordinateur</param>
        public void RemoveMiniature(int id, string computername)
        {
            foreach (Miniature miniature in MiniatureList)
            {
                if (miniature.StudentID == id && miniature.ComputerName == computername)
                {
                    MiniatureList.Remove(miniature);
                    miniature.Dispose();
                    UpdateAllLocations(MaxWidth);
                    break;
                }
            }
        }
    }
}
