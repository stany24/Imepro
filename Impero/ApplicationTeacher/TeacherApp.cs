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
using System.Linq;

namespace ApplicationTeacher
{
    public partial class TeacherApp : Form
    {
        bool FilterEnabled = true;
        readonly MiniatureDisplayer Displayer;
        readonly List<DataForTeacher> AllStudents = new();
        readonly List<DisplayStudent> AllStudentsDisplay = new();
        Task ScreenSharer;
        bool isAsking = false;
        int NextId = 0;
        IPAddress ipAddr = null;

        public TeacherApp()
        {
            InitializeComponent();
            Displayer = new(panelMiniatures.Width);
            FindIp();
            LoadConfigurationLists();
            Task.Run(AskingData);
            Task.Run(LogClients);
        }

        /// <summary>
        /// Fonction qui charge toutes les fichier de configuration
        /// </summary>
        public void LoadConfigurationLists()
        {
            try
            {
                using StreamReader fichier = new(Configuration.pathToSaveFolder + Configuration.FileNameIgnoredProcesses);
                Configuration.IgnoredProcesses = new(JsonSerializer.Deserialize<List<string>>(fichier.ReadToEnd()));
                fichier.Close();
            }
            catch (Exception e)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Problème au chargement de la list des processus ignorées : " + e.ToString()); }));
            }
            try
            {
                using StreamReader fichier = new(Configuration.pathToSaveFolder + Configuration.FileNameAlertedProcesses);
                Configuration.AlertedProcesses = new(JsonSerializer.Deserialize<List<string>>(fichier.ReadToEnd()));
                fichier.Close();
            }
            catch (Exception e)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Problème au chargement de la list des processus alerté : " + e.ToString()); }));
            }
            try
            {
                using StreamReader fichier = new(Configuration.pathToSaveFolder + Configuration.FileNameAlertedUrl);
                Configuration.AlertedUrls = new(JsonSerializer.Deserialize<List<string>>(fichier.ReadToEnd()));
                fichier.Close();
            }
            catch (Exception e)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Problème au chargement de la list des urls alerté  : " + e.ToString()); }));
            }
            try
            {
                using StreamReader fichier = new(Configuration.pathToSaveFolder + Configuration.FileNameAutorisedWebsite);
                Configuration.AutorisedWebsite = new(JsonSerializer.Deserialize<List<string>>(fichier.ReadToEnd()));
                fichier.Close();
            }
            catch (Exception e)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Problème au chargement de la list des urls autorisé  : " + e.ToString()); }));
            }
        }

        /// <summary>
        /// Fonction qui sauvegarde toutes les configuration dans des fichiers
        /// </summary>
        public void SaveConfigurationLists()
        {
            using StreamWriter writeFichierProcesusIgnore = new(Configuration.pathToSaveFolder + Configuration.FileNameIgnoredProcesses);
            writeFichierProcesusIgnore.WriteLine(JsonSerializer.Serialize(Configuration.IgnoredProcesses));
            writeFichierProcesusIgnore.Close();
            using StreamWriter writeFichierProcesusAlerte = new(Configuration.pathToSaveFolder + Configuration.FileNameAlertedProcesses);
            writeFichierProcesusAlerte.WriteLine(JsonSerializer.Serialize(Configuration.AlertedProcesses));
            writeFichierProcesusAlerte.Close();
            using StreamWriter writeFichierUrlsAlerte = new(Configuration.pathToSaveFolder + Configuration.FileNameAlertedUrl);
            writeFichierUrlsAlerte.WriteLine(JsonSerializer.Serialize(Configuration.AlertedUrls));
            writeFichierUrlsAlerte.Close();
            using StreamWriter writeFichierAutorisedUrl = new(Configuration.pathToSaveFolder + Configuration.FileNameAutorisedWebsite);
            writeFichierAutorisedUrl.WriteLine(JsonSerializer.Serialize(Configuration.AutorisedWebsite));
            writeFichierAutorisedUrl.Close();
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
                    Task.Run(() => SendAutorisedUrl(clientSocket));
                    NextId++;
                }
                catch (Exception e) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(e.ToString()); })); }
            }
        }

        /// <summary>
        /// Fonction qui envoye les url autorisé a un client.
        /// </summary>
        /// <param name="socket"></param>
        public void SendAutorisedUrl(Socket socket)
        {
            while (isAsking) {Thread.Sleep(100);}
            isAsking = true;
            socket.Send(Encoding.Default.GetBytes("url"));
            //serialization
            string jsonString = JsonSerializer.Serialize(Configuration.AutorisedWebsite);
            //envoi
            Thread.Sleep(100);
            socket.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
            isAsking = false;
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
                    while (isAsking) { Thread.Sleep(10); }
                    isAsking = true;
                    DateTime StartUpdate = DateTime.Now;
                    DateTime NextUpdate = DateTime.Now.AddSeconds(Configuration.DurationBetweenDemand);
                    List<DataForTeacher> ClientToRemove = new();
                    for (int i = 0; i < AllStudents.Count; i++)
                    {
                        Socket socket = AllStudents[i].SocketToStudent;
                        socket.ReceiveTimeout = Configuration.DefaultTimeout;
                        socket.SendTimeout = Configuration.DefaultTimeout;
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
                            if(display.Student.ID == student.ID)
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
                        isAsking = false;
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
                TreeNode[] students = TreeViewDetails.Nodes.Find(Convert.ToString(student.UserName), false);
                TreeNode[] computers = students[0].Nodes.Find(student.ComputerName,false);
                if(computers.Length > 0) { computers[0].Remove(); }
                if (students[0].Nodes.Count == 0) { students[0].Remove(); }
            }));
            TreeViewSelect.Invoke(new MethodInvoker(delegate {
                TreeNode[] students = TreeViewSelect.Nodes.Find(Convert.ToString(student.UserName), false);
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
                int id = student.ID;
                byte[] dataBuffer = new byte[100000];
                socket.ReceiveTimeout = Configuration.DefaultTimeout;
                int nbData = socket.Receive(dataBuffer);
                Array.Resize(ref dataBuffer, nbData);
                Data data = JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer));
                student = new(data)
                {
                    SocketToStudent = socket,
                    ID = id
                };
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
                socket.ReceiveTimeout = Configuration.DefaultTimeout;
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
                //Trouver ou créer la node pour l'élève
                TreeNode nodeStudent;
                try { nodeStudent = TreeViewDetails.Nodes.Find(Convert.ToString(student.UserName), false)[0]; }
                catch { nodeStudent = TreeViewDetails.Nodes.Add(student.UserName, student.UserName); }
                //Trouver ou créer la node pour le pc
                TreeNode nodeComputer;
                try { nodeComputer = nodeStudent.Nodes.Find(Convert.ToString(student.ID),false)[0]; }
                catch { nodeComputer = nodeStudent.Nodes.Add(Convert.ToString(student.ID), student.ComputerName); }
                //Trouver ou créer la node pour les processus du pc
                TreeNode nodeProcess;
                try{ nodeProcess = nodeComputer.Nodes[0]; }
                catch { nodeProcess = nodeComputer.Nodes.Add("Processus:"); }
                //Trouver ou créer la node pour les urls du pc
                TreeNode nodeNavigateurs;
                try { nodeNavigateurs = nodeComputer.Nodes[1]; }
                catch { nodeNavigateurs = nodeComputer.Nodes.Add("Navigateurs:"); }
                //Mise à jour des processus
                nodeProcess.Nodes.Clear();
                foreach(KeyValuePair<int,string> process in student.Processes) {
                    TreeNode current = nodeProcess.Nodes.Add(process.Value);
                    if (FilterEnabled)
                    {
                        if (Configuration.AlertedProcesses.Find(x => x == process.Value) != null)
                        {
                            current.BackColor = Color.Red;
                            while (current.Parent != null)
                            {
                                current = current.Parent;
                                current.BackColor = Color.Red;
                            }
                        }
                    }
                    else{if(Configuration.IgnoredProcesses.Find(x => x == process.Value) != null){current.BackColor = Color.Yellow;/*current.Remove();*/ }}
                }
                //Mise à jour des urls
                UpdateUrlsTree(nodeNavigateurs, student.Urls.chrome, "chrome","Chrome");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.firefox, "firefox","Firefox");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.edge, "msedge", "Edge");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.opera, "opera", "Opera");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.iexplorer, "iexplorer", "Internet Explorer");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.safari, "safari", "Safari");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.custom, "custom", "Custom");
            }));
            // Mise à jour du TreeView pour la sélection
            TreeViewSelect.Invoke(new MethodInvoker(delegate {
                //Trouver ou créer la node pour l'élève
                TreeNode nodeStudent;
                try { nodeStudent = TreeViewSelect.Nodes.Find(Convert.ToString(student.UserName), false)[0]; }
                catch { nodeStudent = TreeViewSelect.Nodes.Add(student.UserName, student.UserName); }
                //Trouver ou créer la node pour le pc
                TreeNode nodeComputer;
                try { nodeComputer = nodeStudent.Nodes.Find(Convert.ToString(student.ID), false)[0]; }
                catch { nodeComputer = nodeStudent.Nodes.Add(Convert.ToString(student.ID), student.ComputerName); }
            }));
        }

        /// <summary>
        /// Fonction qui met à jour les urls montré dans le TreeView
        /// </summary>
        /// <param name="NodeAllNavigateur">TreeNode qui contient tout les navigateurs</param>
        /// <param name="urls">List des urls pour ce navigateur</param>
        /// <param name="ProcessName">Nom du processus pour ce navigateur</param>
        /// <param name="DisplayName">Nom d'affichage pour ce navigateur</param>
        /// <returns></returns>
        public void UpdateUrlsTree(TreeNode NodeAllNavigateur, List<Url> urls, string ProcessName, string DisplayName) {
            if(urls.Count == 0) { try { NodeAllNavigateur.Nodes.Find(ProcessName, false)[0].Remove(); } catch { }; return; }
            TreeViewDetails.Invoke(new MethodInvoker(delegate {
                TreeNode[] nodeNavigateur = NodeAllNavigateur.Nodes.Find(ProcessName, false);
                if (nodeNavigateur.Count() == 0 ) {NodeAllNavigateur.Nodes.Add(DisplayName,ProcessName);}
                TreeNode NodeBrowser = NodeAllNavigateur.Nodes.Find(ProcessName, false)[0]; ;
                for (int i = NodeAllNavigateur.Nodes.Find(ProcessName, false)[0].Nodes.Count; i < urls.Count; i++)
                {
                    TreeNode NodeUrl = NodeBrowser.Nodes.Add(urls[i].ToString());
                }
                if (FilterEnabled)
                {
                    for (int i = 0; i < NodeBrowser.Nodes.Count; i++)
                    {
                        for (int j = 0; j < Configuration.AlertedUrls.Count; j++)
                        {
                            if (NodeBrowser.Nodes[i].Text.ToLower().Contains(Configuration.AlertedUrls[j]))
                            {
                                TreeNode NodeUrl = NodeBrowser.Nodes[i];
                                NodeUrl.BackColor = Color.Red;
                                while (NodeUrl.Parent != null)
                                {
                                    NodeUrl = NodeUrl.Parent;
                                    NodeUrl.BackColor = Color.Red;
                                }
                            }
                        }
                    }

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
                    if (students.ComputerName == e.Node.Text) { student = students; }
                }
                if (student == null) { return; }
                foreach(Miniature mini in Displayer.MiniatureList) { if (mini.ComputerName == student.ComputerName && mini.StudentID == student.ID) { return; } }
                Miniature miniature = new(student.ScreenShot, student.ComputerName, student.ID, Configuration.pathToSaveFolder);
                Displayer.AddMiniature(miniature);
                panelMiniatures.Controls.Add(miniature);
                panelMiniatures.Controls.SetChildIndex(miniature, 0);
            }
            else
            {
                Displayer.RemoveMiniature(Convert.ToInt32(e.Node.Name),e.Node.Text);
            }
        }

        /// <summary>
        /// Fonction qui permet de capturer l'écran et de le partager en multicast
        /// </summary>
        public void RecordAndStreamScreen()
        {
            foreach (DataForTeacher student in Configuration.StudentToShareScreen) { student.SocketToStudent.Send(Encoding.ASCII.GetBytes("receive")); }

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
        /// Fonction qui démmare ou arête le streaming en multicast de l'écran
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShareScreen(object sender, EventArgs e)
        {
            if(ScreenSharer == null)
            {
                ChooseStudentToShareScreen prompt = new(AllStudents);
                if (prompt.ShowDialog(this) == DialogResult.OK)
                {
                    if (Configuration.StudentToShareScreen.Count != 0)
                    {
                        ScreenSharer = Task.Run(RecordAndStreamScreen);
                        btnShare.Text = "Stop Sharing";
                    }
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
            SaveConfigurationLists();
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
        /// Fonction pour changer le zoom des miniatures avec un slider
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Scroll(object sender, EventArgs e)
        {
            Displayer.zoom = Slider.Value / 100.0;
            Displayer.ChangeZoom();
        }

        /// <summary>
        /// Fonction qui vérifie que la node cliquée est bien un ordinateur puis crée un affichage individuel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //vérification que la node est un ordinateur
            if(e.Node== null) return;
            if(e.Node.Parent == null) return;
            if (e.Node.Parent.Parent != null) return;
            foreach(DataForTeacher student in AllStudents)
            {
                if(student.ID == Convert.ToInt32(e.Node.Name)) { OpenPrivateDisplay(student);return; }
            }
        }

        /// <summary>
        /// Fonction qui crée un nouvelle affichage individuel pour un élève
        /// </summary>
        /// <param name="student"></param>
        public void OpenPrivateDisplay(DataForTeacher student)
        {
            foreach (DisplayStudent display in AllStudentsDisplay)
            {
                if (display.Student.ID == student.ID) { return; }
            }
            DisplayStudent newDisplay = new();
            AllStudentsDisplay.Add(newDisplay);
            newDisplay.UpdateAffichage(student);
            newDisplay.FormClosing += new FormClosingEventHandler(RemovePrivateDisplay);
            newDisplay.Show();
        }

        /// <summary>
        /// Fonction qui retire l'affichage individuel lorsque il se ferme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemovePrivateDisplay(object sender, FormClosingEventArgs e)
        {
            DisplayStudent closingDisplay = (DisplayStudent)sender;
            AllStudentsDisplay.Remove(closingDisplay);
        }

        /// <summary>
        /// Fonction qui met à jour les miniatures lorsque le panel est redimensionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelMiniatures_Resize(object sender, EventArgs e)
        {
            Displayer.UpdateAllLocations(panelMiniatures.Width);
        }

        /// <summary>
        /// Fonction qui active ou désactive les filtres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            FilterEnabled = !FilterEnabled;
            if(FilterEnabled)
            {
                btnFilter.Text = "Désactiver";
                foreach(DataForTeacher student in AllStudents)
                {
                    UpdateTreeViews(student);
                }
            }
            else {
                btnFilter.Text = "Activer";
                foreach (TreeNode node in TreeViewDetails.Nodes)
                {
                    RemoveFilter(node);
                }
            }
        }

        /// <summary>
        /// Fonction qui retire la couleur de fond de toutes les TreeNode
        /// </summary>
        /// <param name="node"></param>
        void RemoveFilter(TreeNode node)
        {
            node.BackColor= Color.White;
            foreach (TreeNode subnode in node.Nodes)
            {
                RemoveFilter(subnode);
            }
        }
    }
}
