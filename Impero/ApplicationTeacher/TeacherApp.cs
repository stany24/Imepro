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
        bool isSharing = false;
        bool isAsking = false;
        int NextId = 0;
        IPAddress ipAddr = null;

        public TeacherApp()
        {
            InitializeComponent();
            Displayer = new(panelMiniatures.Width);
            FindIp();
            Task.Run(StartTasks);
        }

        public void StartTasks()
        {
            while (!IsHandleCreated) { Thread.Sleep(10); }
            LoadConfigurationLists();
            Task.Run(AskingData);
            Task.Run(LogClients);
        }

        /// <summary>
        /// Fonction qui charge toutes les fichier de configuration
        /// </summary>
        public void LoadConfigurationLists()
        {
            LoadFileToList(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameIgnoredProcesses,ref ConfigurationStatic.IgnoredProcesses, lbxConnexion);
            LoadFileToList(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameAlertedProcesses,ref ConfigurationStatic.AlertedProcesses, lbxConnexion);
            LoadFileToList(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameAlertedUrl,ref ConfigurationStatic.AlertedUrls, lbxConnexion);
            LoadFileToList(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameAutorisedWebsite,ref ConfigurationStatic.AutorisedWebsite, lbxConnexion);
        }

        /// <summary>
        /// Fonction qui permet de charger un fichier dans une liste
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="list"></param>
        /// <param name="lbxOutput"></param>
        public void LoadFileToList(string pathToFile, ref List<string> list, ListBox lbxOutput)
        {
            try
            {
                if (!Directory.Exists(ConfigurationStatic.pathToSaveFolder)) {Directory.CreateDirectory( ConfigurationStatic.pathToSaveFolder);}
                if (!File.Exists(pathToFile)) {File.WriteAllText(pathToFile, "[]");}
                list = new(JsonSerializer.Deserialize<List<string>>(File.ReadAllText(pathToFile)));
            }
            catch (Exception e)
            {
                lbxOutput.Invoke(new MethodInvoker(delegate { lbxOutput.Items.Add("Problème au chargement de la list : " + e.ToString()); }));
            }
        }

        /// <summary>
        /// Fonction qui permet de sauvgareder une liste dans un fichier
        /// </summary>
        /// <param name="pathToFile"></param>
        /// <param name="list"></param>
        public void SaveListToFile(string pathToFile, List<string> list, ListBox lbxOutput)
        {
            try
            {
                if (!File.Exists(pathToFile)){File.Create(pathToFile );}
                using StreamWriter writeFichierProcesusIgnore = new(pathToFile);
                writeFichierProcesusIgnore.WriteLine(JsonSerializer.Serialize(list));
                writeFichierProcesusIgnore.Close();
            }
            catch (Exception e)
            {
                lbxOutput.Invoke(new MethodInvoker(delegate { lbxOutput.Items.Add("Problème au chargement de la list : " + e.ToString()); }));
            }
        }

        /// <summary>
        /// Fonction qui sauvegarde toutes les configuration dans des fichiers
        /// </summary>
        public void SaveConfigurationLists()
        {
            SaveListToFile(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameIgnoredProcesses, ConfigurationStatic.IgnoredProcesses, lbxConnexion);
            SaveListToFile(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameAlertedProcesses, ConfigurationStatic.AlertedProcesses, lbxConnexion);
            SaveListToFile(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameAlertedUrl, ConfigurationStatic.AlertedUrls, lbxConnexion);
            SaveListToFile(ConfigurationStatic.pathToSaveFolder + ConfigurationStatic.FileNameAutorisedWebsite, ConfigurationStatic.AutorisedWebsite, lbxConnexion);
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
            string jsonString = JsonSerializer.Serialize(ConfigurationStatic.AutorisedWebsite);
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
                    DateTime NextUpdate = DateTime.Now.AddSeconds(ConfigurationStatic.DurationBetweenDemand);
                    List<DataForTeacher> ClientToRemove = new();
                    UpdateEleves(ClientToRemove);
                    foreach (DataForTeacher client in ClientToRemove)
                    {
                        RemoveStudent(client);
                    }
                    UpdateAllIndividualDisplay();
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
        /// Fonction qui met à jour tous les affichage individuels
        /// </summary>
        public void UpdateAllIndividualDisplay()
        {
            foreach (DisplayStudent display in AllStudentsDisplay)
            {
                foreach (DataForTeacher student in AllStudents)
                {
                    if (display.Student.ID == student.ID)
                    {
                        display.UpdateAffichage(student);
                    }
                }
            }
        }

        /// <summary>
        /// Fonction qui fais les demandes à un élèves pour ces données et son image
        /// </summary>
        /// <param name="ClientToRemove"></param>
        public void UpdateEleves(List<DataForTeacher> ClientToRemove)
        {
            for (int i = 0; i < AllStudents.Count; i++)
            {
                Socket socket = AllStudents[i].SocketToStudent;
                socket.ReceiveTimeout = ConfigurationStatic.DefaultTimeout;
                socket.SendTimeout = ConfigurationStatic.DefaultTimeout;
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
                catch (SocketException)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    ClientToRemove.Add(AllStudents[i]);
                }
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
                TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false)[0].Remove();
            }));
            TreeViewSelect.Invoke(new MethodInvoker(delegate {
                TreeViewSelect.Nodes.Find(Convert.ToString(student.ID), false)[0].Remove();
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
                socket.ReceiveTimeout = ConfigurationStatic.DefaultTimeout;
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
                socket.ReceiveTimeout = ConfigurationStatic.DefaultTimeout;
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
                try { nodeStudent = TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false)[0]; }
                catch { nodeStudent = TreeViewDetails.Nodes.Add(Convert.ToString(student.ID),student.UserName+" : "+student.ComputerName); }
                //Trouver ou créer la node pour les processus du pc
                TreeNode nodeProcess;
                try{ nodeProcess = nodeStudent.Nodes[0]; }
                catch { nodeProcess = nodeStudent.Nodes.Add("Processus:"); }
                //Trouver ou créer la node pour les urls du pc
                TreeNode nodeNavigateurs;
                try { nodeNavigateurs = nodeStudent.Nodes[1]; }
                catch { nodeNavigateurs = nodeStudent.Nodes.Add("Navigateurs:"); }
                //Mise à jour des processus
                nodeProcess.Nodes.Clear();
                foreach(KeyValuePair<int,string> process in student.Processes) {
                    TreeNode current = nodeProcess.Nodes.Add(process.Value);
                    if (FilterEnabled)
                    {
                        if (ConfigurationStatic.AlertedProcesses.Find(x => x == process.Value) != null)
                        {
                            current.BackColor = Color.Red;
                            while (current.Parent != null)
                            {
                                current = current.Parent;
                                current.BackColor = Color.Red;
                            }
                        }
                    }
                    else{if(ConfigurationStatic.IgnoredProcesses.Find(x => x == process.Value) != null){current.BackColor = Color.Yellow;/*current.Remove();*/ }}
                }
                //Mise à jour des urls
                UpdateUrlsTree(nodeNavigateurs, student.Urls.chrome, "chrome","Chrome");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.firefox, "firefox","Firefox");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.seleniumchrome, "seleniumchrome", "Selenium Chrome");
                UpdateUrlsTree(nodeNavigateurs, student.Urls.seleniumfirefox, "seleniumfirefox", "Selenium Firefox");
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
                try { nodeStudent = TreeViewSelect.Nodes.Find(Convert.ToString(student.ID), false)[0]; }
                catch { nodeStudent = TreeViewSelect.Nodes.Add(Convert.ToString(student.ID), student.UserName + " : " + student.ComputerName); }
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
                if (nodeNavigateur.Count() == 0 ) {NodeAllNavigateur.Nodes.Add(ProcessName,DisplayName);}
                TreeNode NodeBrowser = NodeAllNavigateur.Nodes.Find(ProcessName, false)[0]; ;
                for (int i = NodeAllNavigateur.Nodes.Find(ProcessName, false)[0].Nodes.Count; i < urls.Count; i++)
                {
                    NodeBrowser.Nodes.Add(urls[i].ToString());
                }
                if (FilterEnabled)
                {
                    ApplyUrlFilter(NodeBrowser);
                }
            }));
        }

        /// <summary>
        /// Fonction qui active les filtre dans les TreeViews
        /// </summary>
        /// <param name="NodeBrowser"></param>
        public void ApplyUrlFilter(TreeNode NodeBrowser)
        {
            for (int i = 0; i < NodeBrowser.Nodes.Count; i++)
            {
                for (int j = 0; j < ConfigurationStatic.AlertedUrls.Count; j++)
                {
                    if (NodeBrowser.Nodes[i].Text.ToLower().Contains(ConfigurationStatic.AlertedUrls[j]))
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

        /// <summary>
        /// Fonction qui permet la création ou la supresion de miniatures par rapport aux checkbox. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodeChecked(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) { return; } // no node selected
            if (e.Node.Checked)
            {
                DataForTeacher student = null;
                foreach (DataForTeacher students in AllStudents)
                {
                    if (Convert.ToString(students.ID) == e.Node.Name) { student = students; }
                }
                if (student == null) { return; }
                foreach(Miniature mini in Displayer.MiniatureList) { if (mini.ComputerName == student.ComputerName && mini.StudentID == student.ID) { return; } }
                Miniature miniature = new(student.ScreenShot, student.ComputerName, student.ID, ConfigurationStatic.pathToSaveFolder);
                Displayer.AddMiniature(miniature);
                panelMiniatures.Controls.Add(miniature);
                panelMiniatures.Controls.SetChildIndex(miniature, 0);
            }
            else
            {
                Displayer.RemoveMiniature(Convert.ToInt32(e.Node.Name));
            }
        }

        /// <summary>
        /// Fonction qui permet de capturer l'écran et de le partager en multicast
        /// </summary>
        public void RecordAndStreamScreen()
        {
            foreach (DataForTeacher student in ConfigurationStatic.StudentToShareScreen) { student.SocketToStudent.Send(Encoding.ASCII.GetBytes("receive")); }

            Socket s = new(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.AddMembership, new MulticastOption(ip));
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.Parse("232.1.2.3").GetAddressBytes());
            s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.MulticastTimeToLive, 10);
            IPEndPoint ipep = new(ip, 45678);
            s.Connect(ipep);
            Thread.Sleep(1000);

            while (isSharing)
            {
                Screen screen = Screen.AllScreens[ConfigurationStatic.ScreenToShareIndex];
                Bitmap bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
                Rectangle ScreenSize = screen.Bounds;
                Graphics.FromImage(bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
                ImageConverter converter = new();
                byte[] imageArray = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                CustomMessageSender Sender = new(imageArray);
                foreach(CustomMessage message in Sender.GetMessages())
                {
                    s.Send(message.GetContent().ToArray());
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
            if(isSharing == false)
            {
                ChooseStreamOptions prompt = new(AllStudents);
                if (prompt.ShowDialog(this) != DialogResult.OK){return;}
                if (ConfigurationStatic.StudentToShareScreen.Count == 0){return;}
                isSharing= true;
                SendStreamConfiguration();
                ScreenSharer = Task.Run(RecordAndStreamScreen);
                btnShare.Text = "Stop Sharing";
            }
            else
            {
                isSharing = false;
                for (int i = 0; i < ConfigurationStatic.StudentToShareScreen.Count; i++)
                {
                    try
                    {
                        ConfigurationStatic.StudentToShareScreen[i].SocketToStudent.Send(Encoding.Default.GetBytes("stops"));
                    }
                    catch { }
                    ConfigurationStatic.StudentToShareScreen.Remove(ConfigurationStatic.StudentToShareScreen[i]);
                }
                btnShare.Text = "Share screen";
                ScreenSharer.Wait();
                ScreenSharer.Dispose();
            }
        }

        /// <summary>
        /// Fonction qui envoie les configurations du stream aux élèves concernés
        /// </summary>
        private void SendStreamConfiguration()
        {
            byte[] bytes = Encoding.Default.GetBytes(JsonSerializer.Serialize(ConfigurationStatic.streamoptions));
            foreach(DataForTeacher student in ConfigurationStatic.StudentToShareScreen)
            {
                student.SocketToStudent.Send(Encoding.Default.GetBytes("apply"));
            }
            Thread.Sleep(100);
            foreach (DataForTeacher student in ConfigurationStatic.StudentToShareScreen)
            {
                student.SocketToStudent.Send(bytes);
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
                try{student.SocketToStudent.Send(Encoding.Default.GetBytes("stops"));}catch { }
                try { student.SocketToStudent.Send(Encoding.ASCII.GetBytes("disconnect")); } catch { }
                student.SocketToStudent.Dispose();
                //student.SocketToStudent.Disconnect(false);
            }
            TrayIconTeacher.Visible = false;
            TrayIconTeacher.Dispose();
            SaveConfigurationLists();
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
            DisplayStudent newDisplay = new(ipAddr);
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

        /// <summary>
        /// Fonction qui minimise tous les noeuds des treeveiw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideTreeView_Click(object sender, EventArgs e)
        {
            TreeNodeCollection nodes = TreeViewDetails.Nodes;
            foreach(TreeNode node in nodes)
            {
                node.Collapse(false);
            }
            nodes = TreeViewSelect.Nodes;
            foreach (TreeNode node in nodes)
            {
                node.Collapse(false);
            }
        }

        /// <summary>
        /// Fonction qui ouvre tous les noeuds des treeveiw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTreeView_Click(object sender, EventArgs e)
        {
            TreeNodeCollection nodes = TreeViewDetails.Nodes;
            foreach (TreeNode node in nodes)
            {
                node.ExpandAll();
            }
            nodes = TreeViewSelect.Nodes;
            foreach (TreeNode node in nodes)
            {
                node.ExpandAll();
            }
        }

        private void btnOpenConfigWindow_Click(object sender, EventArgs e)
        {
            ConfigurationWindow configWindow = new ConfigurationWindow();
            configWindow.Show();
        }
    }
}
