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
    /// Class parente, utilisé pour les données qui sont transferé de l'élève au professeur
    /// </summary>
    [Serializable]
    public class Data
    {
        #region Variables

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

        #endregion

        #region Constructor

        public Data(string userName, string computerName, HistoriqueUrls urls, Dictionary<int, string> processes)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
        }

        public Data() { }

        #endregion
    }

    /// <summary>
    /// Classe qui représente un élève dans l'application professeur
    /// </summary>
    public class DataForTeacher : Data
    {
        #region Variables

        public Socket SocketToStudent;
        public Socket SocketControl = null;
        public int ID;
        public int NumberOfFailure;

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
        readonly private List<string> DefaultProcess = new();
        readonly private ListBox lbxConnexion;
        readonly private PictureBox pbxScreenShot;
        public IPAddress IpToTeacher;
        readonly private ListBox tbxMessage;
        readonly private Form form;
        public List<string> AutorisedUrls = new();
        readonly private List<string> browsersList = new() { "chrome", "firefox", "iexplore", "safari", "opera", "msedge" };
        public List<int> SeleniumProcessesID = new();
        public DataForStudent(ListBox lbxconnexion, PictureBox pbxscreenshot, ListBox tbxmessage, Form form)
        {
            lbxConnexion = lbxconnexion;
            pbxScreenShot = pbxscreenshot;
            tbxMessage = tbxmessage;
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
                        Process parent = GetParent(instance);
                        if(parent != null)
                        {
                            if (parent.ProcessName == singleBrowser) { continue; }
                        }
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
            Graphics.FromImage(Bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
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
        /// Fonction qui connecte cette application à l'application du professeur
        /// </summary>
        public Socket ConnectToTeacher(int port)
        {
            try
            {
                // Establish the remote endpoint for the socket. This example uses port 11111 on the local computer.
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
            Socket SocketControl = ConnectToTeacher(11112);
            while (isControled)
            {
                SendImage(TakeSreenShot(Screen.AllScreens[screenToStream]), SocketControl);
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
            Task.Run(MinimizeUnAutorisedEverySecond);
            SocketMulticast = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 45678);
            SocketMulticast.Bind(ipep);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            SocketMulticast.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.SetBuffer(new byte[65000], 0, 65000);
            receiveArgs.Completed += ReceiveCompleted;
            BeginReceive(receiveArgs);
        }

        Socket SocketMulticast;
        SocketAsyncEventArgs receiveArgs;
        readonly List<byte> byteImage = new();

        private void BeginReceive(SocketAsyncEventArgs args)
        {
            isReceiving = true;
            bool willRaiseEvent = SocketMulticast.ReceiveAsync(args);
            if (!willRaiseEvent)
                ProcessReceive(args);
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            ProcessReceive(args);
        }

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
        /// Fonction qui recoit les paramêtre du stream et les appliques
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
            gkh.Hook();
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
                WindowMinimize.MinimizeUnAuthorised(options.focus);
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
}
