using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryData;
using System.Text;
using System.Drawing;
using System.Text.Json;
using System.IO;
using System.Net.NetworkInformation;
using System.Linq;

namespace ApplicationCliente
{
    public partial class StudentApp : Form
    {
        readonly DataForStudent Client = new();
        IPAddress IpToTeacher;
        readonly string pathToConfFolder = "C:\\Users\\gouvernonst\\Downloads\\";
        readonly string FileNameConfIp = "iPToTeacher.txt";
        public StudentApp()
        {
            InitializeComponent();
            LoadTeacherIP();
            Task.Run(ConnectToMaster);
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
                while (Client.SocketToTeacher == null)
                {
                    // Si l'addresse du professeur a changé on adapte le socket
                    if(localEndPoint.Address != IpToTeacher)
                    {
                        localEndPoint.Address = IpToTeacher;
                        sender = new(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    }
                    try
                    {
                        // Connect Socket to the remote endpoint using method Connect()
                        sender.Connect(localEndPoint);
                        Client.SocketToTeacher = sender;
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
                try { lenght = Client.SocketToTeacher.Receive(info); }
                catch (SocketException) { return; }
                Array.Resize(ref info, lenght);
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(Encoding.Default.GetString(info)); }));
                string text = Encoding.Default.GetString(info);
                switch (Encoding.Default.GetString(info).Split(' ')[0])
                {
                    case "data": SendData(); break;
                    case "image": SendImage(); break;
                    //case "kill": KillSelectedProcess(Convert.ToInt32(text.Split(' ')[1])); break;
                    case "receive": Task.Run(() => ReceiveMulticastStream()); break;
                    case "stop": 
                        Client.SocketToTeacher.Disconnect(false);
                        Client.SocketToTeacher = null;
                        lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Le professeur a coupé la connexion"); }));
                        Thread.Sleep(1000);
                        Task.Run(() => ConnectToMaster());
                        
                        return;
                }
            }
        }

        /// <summary>
        /// Fonction qui sérialize les données puis les envoient au professeur
        /// </summary>
        private void SendData()
        {
            Client.GetCurrentWebTabsName();
            Client.GetUserProcesses();
            //serialization
            string jsonString = JsonSerializer.Serialize(Client.ToData(), new JsonSerializerOptions { IncludeFields = true, });
            //envoi
            Client.SocketToTeacher.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
        }

        /// <summary>
        /// Fonction qui envoie le screenshot au professeur
        /// </summary>
        private void SendImage()
        {
            Client.TakeScreenShot();
            byte[] image;
            ImageConverter converter = new();
            image = (byte[])converter.ConvertTo(Client.ScreenShot, typeof(byte[]));
            Client.SocketToTeacher.Send(image, 0, image.Length, SocketFlags.None);
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
        /// Fonction qui recoit la diffusion multicast envoyée par le professeur
        /// </summary>
        public void ReceiveMulticastStream()
        {
            Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 45678);
            s.Bind(ipep);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership,new MulticastOption(ip, IPAddress.Any));
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
                catch { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Erreur avec l'image "+i); })); }
                
            }
        }

        public void OnClosing(object sender, FormClosedEventArgs e)
        {
            if (Client.SocketToTeacher != null)
            {
                try {
                    Client.SocketToTeacher.Send(Encoding.ASCII.GetBytes("stop"));
                    Client.SocketToTeacher.Disconnect(false);
                    Client.SocketToTeacher = null;}
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
