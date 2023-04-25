using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryData;
using System.Threading;
using System.IO;
using System.Text.Json;
using System.Drawing.Imaging;

namespace ApplicationTeacher
{
    public partial class TeacherApp : Form
    {
        readonly List<DataForTeacher> AllStudents = new();
        readonly List<DisplayStudent> AllStudentsDisplay = new();
        Task ScreenSharer;
        readonly int DurationBetweenDemand = 15;
        readonly int DefaultTimeout = 2000;
        int NextId = 0;
        IPAddress ipAddr = null;
        public TeacherApp()
        {
            InitializeComponent();
            lblStudents.DataSource = AllStudents;
            // Establish the local endpointfor the socket.
            // Dns.GetHostName returns the name of the host running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress address in ipHost.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) { ipAddr = address; break; }
            }
            lblIP.Text = "IP: "+ipAddr.ToString();
            Task.Run(AskingData);
            Task.Run(LogClients);
        }

        /// <summary>
        /// Fonction qui permet de connecter les élèves qui en font la demande
        /// </summary>
        public void LogClients()
        {
            while (IsHandleCreated == false){Thread.Sleep(1);}
            IPEndPoint localEndPoint = new(ipAddr, 11111);
            // Creation TCP/IP Socket using Socket Class Constructor
            Socket listener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Using Bind() method we associate a network address to the Server Socket
            // All client that will connect to this Server Socket must know this network Address
            listener.Bind(localEndPoint);
            // Using Listen() method we create the Client list that will want to connect to Server
            listener.Listen(-1);
            while (true)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Waiting connection..."); }));
                try
                {
                    // Suspend while waiting for incoming connection Using Accept() method the server will accept connection of client
                    Socket clientSocket = listener.Accept();
                    lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("New connexion to " + clientSocket.RemoteEndPoint); }));
                    AllStudents.Add(new DataForTeacher(clientSocket,NextId));
                    NextId++;
                    lbxClients.Invoke(new MethodInvoker(delegate {
                        lblStudents.DisplayMember = "UserName";
                        lblStudents.DataSource = null;
                        lblStudents.DataSource = AllStudents;
                    }));
                }
                catch (Exception e) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(e.ToString()); })); }
            }
        }

        /// <summary>
        /// Fonction qui demande à tous les élèves connectés leurs données ainsi que leur image
        /// </summary>
        private void AskingData()
        {
            while (true)
            {
                if (AllStudents.Count != 0)
                {
                    DateTime StartUpdate = DateTime.Now;
                    DateTime NextUpdate = DateTime.Now.AddSeconds(DurationBetweenDemand);
                    List<DataForTeacher> ClientToRemove = new();
                    for (int i = 0; i < AllStudents.Count; i++)
                    {
                        Socket socket = AllStudents[i].SocketToStudent;
                        socket.ReceiveTimeout = DefaultTimeout;
                        socket.SendTimeout = DefaultTimeout;
                        try
                        {
                            //demande les données
                            socket.Send(Encoding.ASCII.GetBytes("data"));
                            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("asked data to client " + AllStudents[i].UserName); }));
                            Task.Run(() => AllStudents[i] = ReceiveData(AllStudents[i])).Wait();
                            //demande le screenshot
                            socket.Send(Encoding.ASCII.GetBytes("image"));
                            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("asked image to client " + AllStudents[i].UserName); }));
                            Task.Run(() => ReceiveImage(AllStudents[i])).Wait();
                            lbxClients.Invoke(new MethodInvoker(delegate { lbxClients.Items.Add(AllStudents[i].ToString()); }));
                            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = AllStudents[i].ScreenShot; }));
                        }
                        catch
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            ClientToRemove.Add(AllStudents[i]);
                        }
                    }
                    foreach (DataForTeacher client in ClientToRemove)
                    {
                        AllStudents.Remove(client);
                        lbxClients.Invoke(new MethodInvoker(delegate {
                            lblStudents.DisplayMember = "UserName";
                            lblStudents.DataSource = null;
                            lblStudents.DataSource = AllStudents;
                        }));
                        lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("Le client " + client.UserName + "à été retiré"); }));
                    }
                    foreach(DisplayStudent display in AllStudentsDisplay)
                    {
                        foreach(DataForTeacher student in AllStudents)
                        {
                            if(display.StudentId == student.ID)
                            {
                                display.UpdateAffichage(student);
                            }
                        }
                    }

                    //foreach (DataForTeacher client in AllClients) { client.UpdateAffichage(); }
                    DateTime FinishedUpdate = DateTime.Now;
                    TimeSpan UpdateDuration = FinishedUpdate - StartUpdate;
                    TimeSpan CycleDuration = NextUpdate - StartUpdate;
                    if (CycleDuration > UpdateDuration)
                    {
                        lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("Attente du prochain cycle dans " + (CycleDuration - UpdateDuration) + " secondes"); }));
                        Thread.Sleep(CycleDuration - UpdateDuration);
                    }
                }
                else { Thread.Sleep(100); }
            }
        }

        /// <summary>
        /// Fonction qui permet de recevoir les données envoiées par un élève
        /// </summary>
        /// <param name="student">L'élève qui à envoyé les donneés</param>
        /// <returns></returns>
        private DataForTeacher ReceiveData(DataForTeacher student)
        {
            try
            {
                Socket socket = student.SocketToStudent;
                //AffichageEleve affichage = student.affichage;
                byte[] dataBuffer = new byte[1024];
                socket.ReceiveTimeout = DefaultTimeout;
                int nbData = socket.Receive(dataBuffer);
                Array.Resize(ref dataBuffer, nbData);
                student = new(JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer)))
                {
                    SocketToStudent = socket
                };
                //student.affichage = affichage;
                lbxClients.Invoke(new MethodInvoker(delegate {
                    lblStudents.DisplayMember = "UserName";
                    lblStudents.DataSource = null;
                    lblStudents.DataSource = AllStudents;
                }));
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("data recue de " + student.UserName); }));
                return student;
            }
            catch
            {
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(student.UserName + "n'a pas envoyé de donnée"); }));
                return student;
            }
        }

        /// <summary>
        /// Fonction qui recoit l'image qu'un élève a envoyé
        /// </summary>
        /// <param name="student">L'élève qui à envoyé l'image</param>
        private void ReceiveImage(DataForTeacher student)
        {
            try
            {
                Socket socket = student.SocketToStudent;
                byte[] imageBuffer = new byte[10485760];
                socket.ReceiveTimeout = DefaultTimeout;
                int nbData = socket.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
                Array.Resize(ref imageBuffer, nbData);
                student.ScreenShot = new Bitmap(new MemoryStream(imageBuffer));
                lbxClients.Invoke(new MethodInvoker(delegate {
                    lblStudents.DisplayMember = "UserName";
                    lblStudents.DataSource = null;
                    lblStudents.DataSource = AllStudents;
                }));
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("image recue de " + student.UserName); }));
            }
            catch { lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(student.UserName + "n'a pas envoyé d'image"); })); }
        }

        /// <summary>
        /// Fonction qui permet de capturer l'écran et de le partager en multicast
        /// </summary>
        public void RecordAndStreamScreen()
        {
            foreach (DataForTeacher student in AllStudents) { student.SocketToStudent.Send(Encoding.ASCII.GetBytes("receive")); }

            Socket s = new(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership, new MulticastOption(ip));
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.Parse("232.1.2.3").GetAddressBytes());
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.MulticastTimeToLive, 3);
            IPEndPoint ipep = new(ip, 45678);
            s.Connect(ipep);
            Random random= new();

            for (int i = 0; i > -1; i++)
            {
                byte[] message = new byte[65000];
                Screen screen = Screen.AllScreens[1];
                Bitmap bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
                Rectangle ScreenSize = screen.Bounds;
                Graphics.FromImage(bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
                ImageConverter converter = new();
                byte[] imageArray = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                for (int j = 0; j < imageArray.Length/65000+1; j++)
                {
                    try { Array.Copy(imageArray, j * 65000, message, 0, 65000); }
                    catch { 
                        Array.Copy(imageArray, j *65000 , message, 0, imageArray.Length % 65000);
                        Array.Resize(ref message, imageArray.Length % 65000);
                    }
                    if (i % 100 == 0) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(i); })); }
                    s.Send(message, message.Length, SocketFlags.None);
                }
            }
        }

        private void SelectedStudentChanged(object sender, EventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            if (listbox.SelectedItem is not DataForTeacher Student) { return; }
            foreach (DisplayStudent display in AllStudentsDisplay)
            {
                if (display.StudentId == Student.ID) { return; }
            }
            DisplayStudent newDisplay = new();
            AllStudentsDisplay.Add(newDisplay);
            newDisplay.StudentId = Student.ID;
            newDisplay.Show();
        }

        /// <summary>
        /// Fonction qui démmare ou arête le streaming en multicast de l'écran
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShareScreen(object sender, EventArgs e)
        {
            if(ScreenSharer == null)
            {
                if (AllStudents.Count != 0)
                {
                    ScreenSharer = Task.Run(RecordAndStreamScreen);
                    btnShare.Text = "Stop Sharing";
                }
            }
            else
            {
                ScreenSharer.Dispose();
                btnShare.Text = "Share screen";
            }
        }

        /// <summary>
        /// Fonction qui en cas de redimensionement de l'application affiche le TrayIcon si nécessaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TeacherAppResized(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                TrayIconTeacher.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState) { TrayIconTeacher.Visible = false; }
        }

        /// <summary>
        /// Fonction qui en cas de click sur le TrayIcon réaffiche l'application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconTeacherClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
    }
}
