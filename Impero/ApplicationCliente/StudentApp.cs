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
using System.Drawing.Imaging;
using System.Linq;

namespace ApplicationCliente
{
    public partial class StudentApp : Form
    {
        readonly DataForStudent Client = new();
        IPAddress IpToTeacher = IPAddress.Parse("157.26.227.198");
        public StudentApp()
        {
            InitializeComponent();
            Task.Run(ConnectToMaster);
        }

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
                    try
                    {
                        // Connect Socket to the remote endpoint using method Connect()
                        sender.Connect(localEndPoint);
                        Task.Run(WaitForDemand);
                        Client.SocketToTeacher = sender;
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
                    case "receive": Task.Run(() => Receive()); break;
                    case "stop": Client.SocketToTeacher = null; Task.Run(() => ConnectToMaster()); return;
                }
            }
        }

        private void SendData()
        {
            Client.GetUrls();
            Client.GetUserProcesses();
            //serialization
            string jsonString = JsonSerializer.Serialize(Client.ToData(), new JsonSerializerOptions { IncludeFields = true, });
            //envoi
            Client.SocketToTeacher.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
        }

        private void SendImage()
        {
            Client.TakeScreenShot();
            byte[] image;
            ImageConverter converter = new();
            image = (byte[])converter.ConvertTo(Client.ScreenShot, typeof(byte[]));
            Client.SocketToTeacher.Send(image, 0, image.Length, SocketFlags.None);
        }

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

        public void Receive()
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
                Bitmap bitmap = new(new MemoryStream(imageArray));
                pbxScreeShot.Invoke(new MethodInvoker(delegate { pbxScreeShot.Image = bitmap; }));
                if (i % 100 == 0){lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(i); }));}
            }
        }
    }
}
