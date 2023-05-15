using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryData;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Net.NetworkInformation;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;

namespace ApplicationCliente
{
    public partial class StudentApp : Form
    {
        readonly DataForStudent Client;
        IPAddress IpToTeacher;
        readonly string pathToConfFolder = "C:\\Users\\gouvernonst\\Downloads\\";
        readonly string FileNameConfIp = "iPToTeacher.txt";
        IWebDriver firefoxdriver;
        IWebDriver chromedriver;
        public StudentApp()
        {
            InitializeComponent();
            LoadTeacherIP();
            Client = new(lbxConnexion,pbxScreeShot,lbxMessages, IpToTeacher,this);
            Task.Run(Client.ConnectToMaster);
            Task.Run(AutomaticChecker);
        }

        public void NewChrome(object sender, EventArgs e)
        {
            Task.Run(StartChrome);
        }

        public void StartChrome()
        {
            chromedriver = new ChromeDriver();
        }

        public void NewFirefox(object sender, EventArgs e)
        {
            Task.Run(StartFirefox);
        }

        public void StartFirefox()
        {
            firefoxdriver = new FirefoxDriver();
        }

        public void AutomaticChecker()
        {
            while(true)
            {
                GetUrls();
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Fonction qui récupére les urls dans les navigateurs sélénium
        /// </summary>
        public void GetUrls()
        {
            try
            {
                if (chromedriver != null)
                {
                    Client.Urls.AddUrl(new Url(DateTime.Now, "seleniumchrome", chromedriver.Url));
                    bool navigateback = true;
                    foreach (string url in Client.AutorisedUrls)
                    {
                        if (chromedriver.Url.Contains(url)) { navigateback = false; }

                    }
                    if (navigateback) { chromedriver.Navigate().Back(); }
                }
            }
            catch { chromedriver.Dispose(); }
            try
            {
                if (firefoxdriver != null)
                {
                
                    Client.Urls.AddUrl(new Url(DateTime.Now, "seleniumfirefox", firefoxdriver.Url));
                    bool navigateback = true;
                    foreach (string url in Client.AutorisedUrls)
                    {
                        if (firefoxdriver.Url.Contains(url)) { navigateback = false; }

                    }
                    if (navigateback) {
                        string url = firefoxdriver.Url;
                        firefoxdriver.Navigate().Back();
                    }
                }
            }
            catch { firefoxdriver.Dispose(); }
        }

        /// <summary>
        /// Fonction qui demande à l'élève une nouvelle adresse ip pour le maitre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewTeacherIP(object sender, EventArgs e)
        {
            AskIp prompt = new();
            if (prompt.ShowDialog(this) == DialogResult.OK) {
                try
                {
                    IpToTeacher = IPAddress.Parse(prompt.LastVerifiedIp);
                    IpForTheWeek allIP = new();
                    allIP.SetIp(prompt.LastVerifiedIp);
                    using StreamWriter writeFichier = new(pathToConfFolder + FileNameConfIp);
                    writeFichier.WriteLine(JsonSerializer.Serialize(allIP, new JsonSerializerOptions { IncludeFields = true, }));
                    prompt.Close();
                    prompt.Dispose();
                }
                catch { }
            }
        }

        /// <summary>
        /// Fonction qui lit le fichier de configuration où les adresses ip des professeur sont stocké
        /// </summary>
        /// <returns></returns>
        public IpForTheWeek ReadConfIp()
        {
            try { 
                using StreamReader fichier = new(pathToConfFolder + FileNameConfIp);
                IpForTheWeek allIP = new(JsonSerializer.Deserialize<IpForTheWeek>(fichier.ReadToEnd()));
                fichier.Close();
                return allIP;
            } 
            catch(Exception e) {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Unexpected exception : " + e.ToString()); }));
                return null;
            }
        }

        /// <summary>
        /// Fonction qui essaye de charger l'adresse ip du professeur
        /// </summary>
        public void LoadTeacherIP()
        {
            try
            {
                IpForTheWeek allIP = ReadConfIp();
                if (allIP == null) { NewTeacherIP(new object(),new EventArgs()); return; }
                IpToTeacher = IPAddress.Parse(allIP.GetIp());
            }
            catch{NewTeacherIP(new object(), new EventArgs());}
        }

        /// <summary>
        /// Fonction qui vérifie sur toutes les interfaces réseau si le forwarding est activé, et génére un script pour n'activer que l'interface avec le professeur
        /// </summary>
        /// <returns> La commande à executer en powershell pour modifier les interfaces</returns>
        public string CheckInterfaces()
        {
            NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();
            string command = String.Empty;
            foreach(NetworkInterface current in netInterface)
            {
                bool isCorrect = false;
                IPInterfaceProperties properities = current.GetIPProperties();
                foreach(UnicastIPAddressInformation ip in properities.UnicastAddresses.Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    if (IsOnSameNetwork(IpToTeacher,ip.Address,ip.IPv4Mask)){isCorrect = true;}
                }
                //IPv4InterfaceProperties ipv4 = properities.GetIPv4Properties();
                //ipv4.IsForwardingEnabled not working
                if (isCorrect == false) { command += "netsh interface ipv4 set interface \"" + current.Name + "\" forwarding=disable\r\n"; }
                else { command += "netsh interface ipv4 set interface \"" + current.Name + "\" forwarding=enable\r\n"; }
            }
            return command;
        }

        /// <summary>
        /// Fonction qui permet de vérifier si 2 ip sont sur le même réseau ou non
        /// </summary>
        /// <param name="ipAddress1">La première adresse ip</param>
        /// <param name="ipAddress2">La deuxième adresse ip</param>
        /// <param name="subnetMask">Le mask de sous réseau d'une des adresse</param>
        /// <returns></returns>
        public static bool IsOnSameNetwork(IPAddress ipAddress1, IPAddress ipAddress2, IPAddress subnetMask)
        {
            // Convert the IP addresses and subnet mask to byte arrays
            byte[] ipBytes1 = ipAddress1.GetAddressBytes();
            byte[] ipBytes2 = ipAddress2.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            // Compare the network portions of the IP addresses using the subnet mask
            for (int i = 0; i < ipBytes1.Length; i++)
            {
                if ((ipBytes1[i] & subnetMaskBytes[i]) != (ipBytes2[i] & subnetMaskBytes[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Fonction qui aide l'utilisateur à configurer ses interfaces réseaux
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void HelpReceive(object sender, EventArgs e)
        {
            string commande = CheckInterfaces();
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\ActiverInterface.ps1");
            using (StreamWriter fichier = new(path))
            {
                fichier.WriteLine(commande);
                fichier.Close();
            }
            MessageBox.Show("Vos interfaces réseau ne sont pas configurés correctement.\r\n" +
                "1) Lancez une fenêtre windows powershell en administrateur.\r\n" +
                "2) Copiez tout le contenu du fichier " + path + ".\r\n" +
                "3) Collez le tout dans le terminal et executez.\r\n", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        /// <summary>
        /// Fonction qui à la fermeture annonce au professeur d'arreter de lui communiquer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender, FormClosedEventArgs e)
        {
            if (Client.SocketToTeacher != null)
            {
                try {
                    Client.SocketToTeacher.Send(Encoding.ASCII.GetBytes("stop"));
                    Client.SocketToTeacher.Disconnect(false);
                    Client.SocketToTeacher = null;
                    TrayIconStudent.Visible= false;
                    TrayIconStudent.Dispose();
                    firefoxdriver?.Dispose();
                    chromedriver?.Dispose();
                }
                catch { }
            }
        }

        /// <summary>
        /// Fonction qui en cas de redimensionement de l'application affiche le TrayIcon si nécessaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StudentAppResized(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                TrayIconStudent.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState) { TrayIconStudent.Visible = false; }
        }

        /// <summary>
        /// Fonction qui en cas de click sur le TrayIcon réaffiche l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconStudentClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
