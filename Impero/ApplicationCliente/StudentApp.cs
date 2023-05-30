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
        readonly DataForStudent Student;
        readonly string pathToConfigurationFolder = "C:\\ProgramData\\Imepro\\";
        readonly string FileNameConfigurationIp = "iPToTeacher.json";
        IWebDriver FirefoxDriver;
        IWebDriver ChromeDriver;
        public StudentApp()
        {
            InitializeComponent();
            Student = new(lbxConnexion,pbxScreeShot,lbxMessages, this);
            LoadTeacherIP();
            Task.Run(LaunchTasks);
        }

        /// <summary>
        /// Fonction qui attend qui le formulaire soit complétement initialisé avant de lancer les tâches
        /// </summary>
        public void LaunchTasks()
        {
            while (!IsHandleCreated) {Thread.Sleep(100);}
            Student.SocketToTeacher = Task.Run(() => Student.ConnectToTeacher(11111)).Result;
            Task.Run(AutomaticChecker);
        }

        public void NewChrome(object sender, EventArgs e)
        {
            Task.Run(StartChrome);
        }

        public void StartChrome()
        {
            ChromeDriver = new ChromeDriver();
        }

        public void NewFirefox(object sender, EventArgs e)
        {
            Task.Run(StartFirefox);
        }

        public void StartFirefox()
        {
            FirefoxDriver = new FirefoxDriver();
        }

        /// <summary>
        /// Fonction qui récupère les urls des navigateurs sélénium toutes les 2 secondes
        /// </summary>
        public void AutomaticChecker()
        {
            while(true)
            {
                VerifyUrlOfWebDriver((WebDriver)FirefoxDriver,"seleniumfirefox");
                VerifyUrlOfWebDriver((WebDriver)ChromeDriver,"seleniumchorme");
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Fonction qui vérifie l'url actuel et l'ajoute dans l'historique
        /// </summary>
        /// <param name="navigateur"></param>
        /// <param name="navigateurName"></param>
        public void VerifyUrlOfWebDriver(WebDriver navigateur,string navigateurName)
        {
            try
            {
                if (navigateur == null){return;}
                Student.Urls.AddUrl(new Url(DateTime.Now, navigateurName, navigateur.Url));
                bool navigateback = true;
                foreach (string url in Student.AutorisedUrls)
                {
                    if (navigateur.Url.Contains(url)) { navigateback = false; }
                }
                if (navigateback)
                {
                    string url = navigateur.Url;
                    navigateur.Navigate().Back();
                }
            }
            catch { navigateur.Dispose(); }
        }

        /// <summary>
        /// Fonction qui demande à l'élève une nouvelle adresse ip pour le maitre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewTeacherIP(object sender, EventArgs e)
        {
            AskIp prompt = new();
            if (prompt.ShowDialog(this) != DialogResult.OK) { return; }
            try
            {
                Student.IpToTeacher = IPAddress.Parse(prompt.LastVerifiedIp);
                IpForTheWeek allIP = ReadConfIp();
                allIP.SetIp(prompt.LastVerifiedIp);
                using StreamWriter writeFichier = new(pathToConfigurationFolder + FileNameConfigurationIp);
                writeFichier.WriteLine(JsonSerializer.Serialize(allIP, new JsonSerializerOptions { IncludeFields = true, }));
                prompt.Close();
                prompt.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// Fonction qui lit le fichier de configuration où les adresses ip des professeur sont stocké
        /// </summary>
        /// <returns></returns>
        public IpForTheWeek ReadConfIp()
        {
            try {
                if (!Directory.Exists(pathToConfigurationFolder)) { Directory.CreateDirectory(pathToConfigurationFolder); }
                if (!File.Exists(pathToConfigurationFolder + FileNameConfigurationIp)) { File.WriteAllText(pathToConfigurationFolder + FileNameConfigurationIp, "[]"); }
                IpForTheWeek allIP = new(JsonSerializer.Deserialize<IpForTheWeek>(File.ReadAllText(pathToConfigurationFolder + FileNameConfigurationIp)));
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
                Student.IpToTeacher = IPAddress.Parse(allIP.GetIp());
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
                    if (IsOnSameNetwork(Student.IpToTeacher,ip.Address,ip.IPv4Mask)){isCorrect = true;}
                }
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
            if (Student.SocketToTeacher == null){return;}
            try
            {
                Student.SocketToTeacher.Send(Encoding.ASCII.GetBytes("stop"));
                Student.SocketToTeacher.Disconnect(false);
                Student.SocketToTeacher = null;
                FirefoxDriver?.Dispose();
                ChromeDriver?.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// Fonction qui en cas de redimensionement de l'application affiche le TrayIcon si nécessaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StudentAppResized(object sender, EventArgs e)
        {
            switch(WindowState)
            {
                case FormWindowState.Minimized:TrayIconStudent.Visible = true;Hide();
                    break;
                case FormWindowState.Normal: TrayIconStudent.Visible = false;
                    break;
                case FormWindowState.Maximized: TrayIconStudent.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// Fonction qui en cas de click sur le TrayIcon réaffiche l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconStudentClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Fonction qui permet de supprimer toutes les ip sauvegardées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetAllIP_Click(object sender, EventArgs e)
        {
            using StreamWriter writeFichier = new(pathToConfigurationFolder + FileNameConfigurationIp);
            writeFichier.WriteLine(JsonSerializer.Serialize(new IpForTheWeek(), new JsonSerializerOptions { IncludeFields = true, }));
        }
    }
}
