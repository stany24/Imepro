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

namespace ApplicationCliente
{
    public partial class StudentApp : Form
    {
        readonly DataForStudent Client = new();
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
                IPEndPoint localEndPoint = new(IPAddress.Parse("157.26.227.198"), 11111);

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

        public void ScreenReceiver()
        {
            UdpClient udpClient = new(11112);
            udpClient.JoinMulticastGroup(IPAddress.Parse("224.100.0.1"));
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
            // Receive messages
            while (true)
            {
                try
                {
                    byte[] imageBuffer = new byte[10485760];
                    int lastId = 0;
                    do
                    {
                        byte[] message = udpClient.Receive(ref ipEndPoint);
                        for (int i = 0; i < message.Length; i++)
                        {
                            imageBuffer[lastId + i] = message[i];
                        }
                        message.CopyTo(imageBuffer, lastId);
                        lastId += message.Length;
                        //lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(" Received: " + message.Length + " bytes"); }));
                    } while (lastId % 65000 == 0);
                    lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(lastId); }));
                    Bitmap bitmap = new(new MemoryStream(imageBuffer));
                    lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Image OK"); }));
                    pbxScreeShot.Invoke(new MethodInvoker(delegate { pbxScreeShot.Image = bitmap; }));
                }
                catch
                {
                    lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("l'image n'a pas étée recue"); }));
                }
            }
        }

        public void Receive()
        {
            Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram,ProtocolType.Udp);
            IPEndPoint ipep = new(IPAddress.Any, 4567);
            s.Bind(ipep);
            IPAddress ip = IPAddress.Parse("224.5.6.7");
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership,new MulticastOption(ip, IPAddress.Any));
            for (int i = 0; i > -1; i++)
            {
                try
                {
                    byte[] message = new byte[65000];
                    int size = s.Receive(message);
                    Array.Resize(ref message, size);
                    Bitmap bitmap = new(new MemoryStream(message));
                    pbxScreeShot.Invoke(new MethodInvoker(delegate { pbxScreeShot.Image = bitmap; }));
                    if (i % 100 == 0){lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(i); }));}
                }
                catch {
                    lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Error: "+i); }));
                }
            }
        }
    }
}
