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
        readonly MiniatureDisplayer Displayer;
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
            Displayer = new(panelMiniatures.Width);
            FindIp();
            Task.Run(AskingData);
            Task.Run(LogClients);
        }

        /// <summary>
        /// Fonction qui 
        /// </summary>
        public void FindIp()
        {
            // Establish the local endpointfor the socket.
            // Dns.GetHostName returns the name of the host running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> PossiblesIp = new();
            foreach (IPAddress address in ipHost.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) { PossiblesIp.Add(address);}
            }
            switch (PossiblesIp.Count)
            {
                case 0:
                    MessageBox.Show("Aucune addresse ip conforme n'a étée trouvée.\r\n" +
                        "Vérifiez vos connexion aux réseaux.\r\n" +
                        "L'application va ce fermer.","Attention",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    Application.Exit();
                    break;
                case 1:ipAddr = PossiblesIp[0]; break;
                default:
                    AskToChoseIp prompt = new(PossiblesIp);
                    if (prompt.ShowDialog(this) == DialogResult.OK){ipAddr = prompt.ChoosenIp;}
                    else { ipAddr = PossiblesIp[0]; }
                    prompt.Dispose();
                    break;
            }
            lblIP.Text = "IP: " + ipAddr.ToString();
        }

        /// <summary>
        /// Fonction qui permet de connecter les élèves qui en font la demande
        /// </summary>
        public void LogClients()
        {
            while (IsHandleCreated == false){Thread.Sleep(100);}
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
                try
                {
                    // Suspend while waiting for incoming connection Using Accept() method the server will accept connection of client
                    Socket clientSocket = listener.Accept();
                    lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss")+" Nouvelle connexion de: " + clientSocket.RemoteEndPoint); }));
                    AllStudents.Add(new DataForTeacher(clientSocket,NextId));
                    NextId++;
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
                            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande des données à " + AllStudents[i].UserName); }));
                            Task.Run(() => AllStudents[i] = ReceiveData(AllStudents[i])).Wait();
                            //demande le screenshot
                            socket.Send(Encoding.ASCII.GetBytes("image"));
                            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Demande de l'image à " + AllStudents[i].UserName); }));
                            Task.Run(() => ReceiveImage(AllStudents[i])).Wait();
                        }
                        catch ( SocketException)
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            ClientToRemove.Add(AllStudents[i]);
                        }
                    }
                    foreach (DataForTeacher client in ClientToRemove)
                    {
                        RemoveStudent(client);
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
                        lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") +  " Attente du prochain cycle dans " + (CycleDuration - UpdateDuration) + " secondes"); }));
                        Thread.Sleep(CycleDuration - UpdateDuration);
                    }
                }
                else { Thread.Sleep(100); }
            }
        }

        /// <summary>
        /// Fonction qui enléve un élève en cas de déconnection
        /// </summary>
        /// <param name="student">L'élève à enlever</param>
        public void RemoveStudent(DataForTeacher student)
        {
            AllStudents.Remove(student);
            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " L'élève " + student.UserName + " est déconnecté"); }));
            TreeViewDetails.Invoke(new MethodInvoker(delegate {
                TreeNode[] students = TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false);
                TreeNode[] computers = students[0].Nodes.Find(student.ComputerName,false);
                if(computers.Length > 0) { computers[0].Remove(); }
                if (students[0].Nodes.Count == 0) { students[0].Remove(); }
            }));
            TreeViewSelect.Invoke(new MethodInvoker(delegate {
                TreeNode[] students = TreeViewSelect.Nodes.Find(Convert.ToString(student.ID), false);
                TreeNode[] computers = students[0].Nodes.Find(student.ComputerName, false);
                if (computers.Length > 0) { computers[0].Remove(); }
                if (students[0].Nodes.Count == 0) { students[0].Remove(); }
            }));
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
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") +  " Données recue de " + student.UserName); }));
                Task.Run(() => UpdateTreeViews(student));
                student.NumberOfFailure = 0;
                return student;
            }
            catch
            {
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " +student.UserName + "n'a pas envoyé de donnée"); }));
                student.NumberOfFailure++;
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
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Image recue de " + student.UserName); }));
                student.NumberOfFailure = 0;
                Displayer.UpdateMiniature(student.ID,student.ComputerName ,student.ScreenShot);
            }
            catch {
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " "+ student.UserName + "n'a pas envoyé d'image"); }));
                student.NumberOfFailure++;
            }
        }

        /// <summary>
        /// Fonction qui met à jour les TreeViews (détails et sélection)
        /// </summary>
        /// <param name="student">L'élève que l'on met à jour</param>
        public void UpdateTreeViews(DataForTeacher student)
        {
            // Mise à jour du TreeView avec les détails.
            TreeViewDetails.Invoke(new MethodInvoker(delegate {
                TreeNode[] nodeStudent = TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false);
                if (nodeStudent.Length == 0)
                {
                    // Créer nouvelle élève
                    TreeNode nodeNewStudent = TreeViewDetails.Nodes.Add(Convert.ToString(student.ID), student.UserName);
                    TreeNode nodePoste = nodeNewStudent.Nodes.Add(student.ComputerName, student.ComputerName);
                    TreeNode nodeProcesses = nodePoste.Nodes.Add("Processus:");
                    foreach (KeyValuePair<int, string> process in student.Processes) { nodeProcesses.Nodes.Add(Convert.ToString(process.Key), process.Value); }
                    TreeNode nodeUrls = nodePoste.Nodes.Add("Urls:");
                    foreach (string url in student.Urls) { nodeUrls.Nodes.Add(url); }
                }
                else
                {
                    //nouvelle ordinateur pour un élève
                    TreeNode[] nodeComputer = nodeStudent[0].Nodes.Find(student.ComputerName, false);
                    if (nodeComputer.Length == 0)
                    {
                        TreeNode nodePoste = nodeStudent[0].Nodes.Add(student.ComputerName, student.ComputerName);
                        TreeNode nodeProcesses = nodePoste.Nodes.Add("Processus:");
                        foreach (KeyValuePair<int, string> process in student.Processes) { nodeProcesses.Nodes.Add(Convert.ToString(process.Key), process.Value); }
                        TreeNode nodeUrls = nodePoste.Nodes.Add("Urls:");
                        foreach (string url in student.Urls) { nodeUrls.Nodes.Add(url); }
                    }
                    else
                    {
                        //mise à jour d'un ordinateur
                        TreeNode proc = nodeComputer[0].Nodes[0];
                        proc.Nodes.Clear();
                        foreach (KeyValuePair<int, string> process in student.Processes) { proc.Nodes.Add(Convert.ToString(process.Key), process.Value); }
                        TreeNode urls = nodeComputer[0].Nodes[1];
                        urls.Nodes.Clear();
                        foreach (string url in student.Urls) { urls.Nodes.Add(url); }
                    }
                }
            }));
            // Mise à jour du TreeView pour la sélection
            TreeViewSelect.Invoke(new MethodInvoker(delegate {
                TreeNode[] nodeStudent = TreeViewSelect.Nodes.Find(Convert.ToString(student.ID), false);
                if (nodeStudent.Length == 0)
                {
                    // Créer nouvelle élève
                    TreeNode nodeNewStudent = TreeViewSelect.Nodes.Add(Convert.ToString(student.ID), student.UserName);
                    nodeNewStudent.Nodes.Add(student.ComputerName, student.ComputerName);
                }
                else
                {
                    //nouvelle ordinateur pour un élève
                    TreeNode[] nodeComputer = nodeStudent[0].Nodes.Find(student.ComputerName, false);
                    if (nodeComputer.Length == 0) { nodeStudent[0].Nodes.Add(student.ComputerName, student.ComputerName); }
                }
            }));
        }

        /// <summary>
        /// Fonction qui permet la création ou la supresion de miniatures par rapport aux checkbox. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodeChecked(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) { return; } // no node selected
            if (e.Node.Parent == null) { return; } //vérification du niveau de la node
            if (e.Node.Parent.Parent != null) { return; }
            if (e.Node.Checked)
            {
                DataForTeacher student = null;
                foreach (DataForTeacher students in AllStudents)
                {
                    if (students.ComputerName == e.Node.Name) { student = students; }
                }
                if (student == null) { return; }
                foreach(Miniature mini in Displayer.MiniatureList) { if (mini.ComputerName == student.ComputerName && mini.StudentID == student.ID) { return; } }
                Miniature miniature = new(student.ScreenShot, student.ComputerName, student.ID);
                Displayer.AddMiniature(miniature);
                panelMiniatures.Controls.Add(miniature);
                panelMiniatures.Controls.SetChildIndex(miniature, 0);
            }
            else
            {
                Displayer.RemoveMiniature(Convert.ToInt32(e.Node.Parent.Name),e.Node.Name);
            }
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

        /// <summary>
        /// Fonction qui ouvre un affichage individuel pour un éléve.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Fonction qui à la fermeture de l'applcation professeur, le signale aux élèves.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender, FormClosedEventArgs e)
        {
            foreach (DataForTeacher student in AllStudents)
            {
                try { student.SocketToStudent.Send(Encoding.ASCII.GetBytes("stop")); } catch { }
                student.SocketToStudent.Dispose();
                //student.SocketToStudent.Disconnect(false);
            }
        }

        /// <summary>
        /// Fonction qui permet d'arêter la communication avec un client
        /// </summary>
        /// <param name="student"></param>
        public void StopClient(object sender, EventArgs e)
        {
            //if (lbxClients.SelectedItem is not DataForTeacher student) { return; }
            //student.SocketToStudent.Send(Encoding.ASCII.GetBytes("stop"));
            //student.SocketToStudent.Disconnect(false);
        }


        /// <summary>
        /// Fonction qui permet de "zoomer" dans le TreeView en modifiant la taille de la police
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomTreeView(object sender, MouseEventArgs e)
        {
            // Only zoom when the mouse is over the control
            if (!TreeViewDetails.ClientRectangle.Contains(e.Location))
            {return;}

            // Increase or decrease the font size based on the mouse wheel delta
            int fontSizeDelta = e.Delta > 0 ? 1 : -1;
            Font oldFont = TreeViewDetails.Font;
            float newFontSize = oldFont.Size + fontSizeDelta;
            if(newFontSize< 5) { return; }
            if (newFontSize > 60) { return; }
            Font newFont = new(oldFont.FontFamily,newFontSize, oldFont.Style);

            // Apply the new font size and redraw the control
            TreeViewDetails.Font = newFont;
            TreeViewDetails.Invalidate();
        }

        /// <summary>
        /// Fonction pour changer le zoom des miniatures avec un slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Scroll(object sender, EventArgs e)
        {
            Displayer.zoom = Slider.Value / 100.0;
            Displayer.ChangeZoom();
        }
    }
}
