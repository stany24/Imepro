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

    public class DataForTeacher : Data
    {
        public Socket SocketToStudent;
        public int ID;
        public int NumberOfFailure;
        public List<Message> Messages = new();

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
            return UserName +" "+ ComputerName;
        }
    }

    public class DataForStudent : Data
    {
        public Socket SocketToTeacher;
        public List<string> DefaultProcess = new();
        readonly private ListBox lbxConnexion;
        readonly private ListBox lbxAutorisedWebSite;
        readonly private PictureBox pbxScreeShot;
        readonly private IPAddress IpToTeacher;
        readonly private ListBox tbxMessage;
        public List<Message> Messages = new();
        public List<string> AutorisedUrls = new();
        readonly public List<string> browsersList = new() { "chrome", "firefox", "iexplore", "safari", "opera", "msedge" };

        public DataForStudent(ListBox lbxconnexion,PictureBox pbxscreenshot, ListBox tbxmessage ,IPAddress ipToTeacher,ListBox lbxwebsite)
        {
            lbxAutorisedWebSite = lbxwebsite;
            lbxConnexion = lbxconnexion;
            pbxScreeShot = pbxscreenshot;
            tbxMessage = tbxmessage;
            IpToTeacher = ipToTeacher;
            GetDefaultProcesses();
            GetNames();
            Task.Run(GetAllTabNameEvery5Seconds);
        }

        /// <summary>
        /// Fonction qui permet de trouver le nom d'utilisateur et le nom de la machine
        /// </summary>
        public void GetNames()
        {
            ComputerName = Environment.MachineName;
            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// Fonction qui retourne un instance de la classe parente
        /// </summary>
        /// <returns></returns>
        public Data ToData()
        {
            return new Data(UserName, ComputerName, Urls, Processes);
        }

        public void GetAllTabNameEvery5Seconds()
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
        public void GetCurrentWebTabsName()
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
                        if (text.ToString() != "") {
                            Urls.AddUrl(new Url(DateTime.Now, singleBrowser, text.ToString()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fonction qui crée une list de processus lancé par l'ordinateur au démarrage de l'application
        /// </summary>
        public void GetDefaultProcesses()
        {
            foreach (Process process in Process.GetProcesses().OrderBy(x => x.ProcessName)) { DefaultProcess.Add(process.ProcessName); }
        }

        /// <summary>
        /// Fonction qui met à jour la list des processus lancé par l'utilisateur
        /// </summary>
        public void GetUserProcesses()
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
        public void TakeScreenShot()
        {
            int TotalWidth = 0;
            int MaxHeight = 0;
            List<Bitmap> images = new();
            //prend une capture d'écran de tout les écran
            foreach (Screen screen in Screen.AllScreens)
            {
                Bitmap Bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
                Rectangle ScreenSize = screen.Bounds;
                Graphics.FromImage(Bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
                images.Add(Bitmap);
                TotalWidth += ScreenSize.Width;
                if (ScreenSize.Height > MaxHeight) { MaxHeight = ScreenSize.Height; }
            }

            Bitmap FullImage = new(TotalWidth, MaxHeight, PixelFormat.Format32bppArgb);
            Graphics FullGraphics = Graphics.FromImage(FullImage);

            int offsetLeft = 0;
            //Crée une seul image de toutes les captures d'écran
            foreach (Bitmap image in images)
            {
                FullGraphics.DrawImage(image, new Point(offsetLeft, 0));
                offsetLeft += image.Width;
            }
            //FullImage = (new Bitmap(FullImage, new Size(200,200)));
            ScreenShot = FullImage;
            FullGraphics.Dispose();
        }

        /// <summary>
        /// Fonction qui sérialize les données puis les envoient au professeur
        /// </summary>
        public void SendData()
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
        public void SendImage()
        {
            TakeScreenShot();
            byte[] image;
            ImageConverter converter = new();
            image = (byte[])converter.ConvertTo(ScreenShot, typeof(byte[]));
            SocketToTeacher.Send(image, 0, image.Length, SocketFlags.None);
        }

        /// <summary>
        /// Fonction qui connecte cette application à l'application du professeur
        /// </summary>
        public void ConnectToMaster()
        {
            try
            {
                // Establish the remote endpoint for the socket. This example uses port 11111 on the local computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPEndPoint localEndPoint = new(IpToTeacher, 11111);

                // Creation TCP/IP Socket using Socket Class Constructor
                Socket sender = new(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                while (SocketToTeacher == null)
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
                        SocketToTeacher = sender;
                        Task.Run(WaitForDemand);
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Connected"); }));
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
        }

        /// <summary>
        /// Fonction qui attend les demandes du professeur et lance la bonne fonction pour y répondre
        /// </summary>
        public void WaitForDemand()
        {
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
                    case "image": SendImage(); break;
                    //case "kill": KillSelectedProcess(Convert.ToInt32(text.Split(' ')[1])); break;
                    case "receive": Task.Run(ReceiveMulticastStream); break;
                    case "message":ReceiveMessage(); break;
                    case "url":ReceiveAuthorisedUrls(); break;
                    case "stop":
                        SocketToTeacher.Disconnect(false);
                        SocketToTeacher = null;
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Le professeur a coupé la connexion"); }));
                        Thread.Sleep(1000);
                        Task.Run(() => ConnectToMaster());
                        return;
                }
            }
        }

        /// <summary>
        /// Fonction qui recoit la liste des urls autorisés
        /// </summary>
        public void ReceiveAuthorisedUrls()
        {
            byte[] bytemessage = new byte[102400];
            int nbData = SocketToTeacher.Receive(bytemessage);
            Array.Resize(ref bytemessage, nbData);
            AutorisedUrls = JsonSerializer.Deserialize<List<string>>(Encoding.Default.GetString(bytemessage));
            lbxAutorisedWebSite.Invoke(new MethodInvoker(delegate {
                lbxAutorisedWebSite.Items.Clear();
                foreach(string url in AutorisedUrls){lbxAutorisedWebSite.Items.Add(url);}
            }));
        }

        /// <summary>
        /// Fonction qui recoit un message du professeur
        /// </summary>
        public void ReceiveMessage()
        {
            byte[] bytemessage = new byte[1024];
            int nbData = SocketToTeacher.Receive(bytemessage);
            Array.Resize(ref bytemessage, nbData);
            tbxMessage.Invoke(new MethodInvoker(delegate { tbxMessage.Items.Add(DateTime.Now.ToString("hh:mm ")+Encoding.Default.GetString(bytemessage)); }));
        }

        /// <summary>
        /// Fonction qui recoit la diffusion multicast envoyée par le professeur
        /// </summary>
        public void ReceiveMulticastStream()
        {
            Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 45678);
            s.Bind(ipep);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
            for (int i = 0; i > -1; i++)
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
                    catch
                    {
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Error: " + i); }));
                    }
                } while (size == 65000);
                Array.Resize(ref imageArray, lastId);
                try
                {
                    Bitmap bitmap = new(new MemoryStream(imageArray));
                    pbxScreeShot.Invoke(new MethodInvoker(delegate { pbxScreeShot.Image = bitmap; }));
                    if (i % 100 == 0) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(i); })); }
                }
                catch { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Erreur avec l'image " + i); })); }

            }
        }
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
                case "chrome": VerifyUrl(chrome, url);
                    break;
                case "firefox":VerifyUrl(firefox, url);
                    break;
                case "opera":VerifyUrl(opera, url);
                    break;
                case "msedge":VerifyUrl(edge, url);
                    break;
                case "safari":VerifyUrl(safari, url);
                    break;
                case "iexplorer":VerifyUrl(iexplorer, url);
                    break;
                case "custom":VerifyUrl(custom, url);
                    break;
                default:break;
            }
        }
        public void VerifyUrl(List<Url> list, Url url)
        {
            if(list.Count == 0) { list.Add(url);return; }
            if(list.Last().Name != url.Name) { list.Add(url); return; }
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

        public Url(DateTime capturetime, string browser,string name)
        {
            CaptureTime = capturetime;
            Browser = browser;
            Name = name;
        }
        public override string ToString()
        {
            return CaptureTime.ToString("HH:mm:ss") +" " +Name ;
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
            lundi= copy.lundi;
            mardi= copy.mardi;
            mercredi= copy.mercredi;
            jeudi= copy.jeudi;
            vendredi= copy.vendredi;
            samedi= copy.samedi;
            dimanche= copy.dimanche;
        }

        /// <summary>
        /// Fonction qui enregistre l'ip donnée au bonne endroit, qui dépand du jour et de l'heure de l'action
        /// </summary>
        /// <param name="ip"></param>
        public void SetIp(string ip)
        {
            try{IPAddress.Parse(ip);}
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
}