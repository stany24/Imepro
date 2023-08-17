using LibraryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class TeacherApp : Form
    {
        #region Variables

        readonly MiniatureDisplayer Displayer;
        readonly List<DataForTeacher> AllStudents = new();
        private List<DataForTeacher> StudentToShareScreen = new();
        readonly List<DisplayStudent> AllStudentsDisplay = new();
        Task ScreenSharer;
        bool isSharing = false;
        bool isAsking = false;
        int NextId = 0;
        IPAddress ipAddr = null;

        #endregion

        #region At start

        public TeacherApp()
        {
            InitializeComponent();
            Displayer = new(panelMiniatures.Width);
            FindIp();
            Task.Run(StartTasks);
        }

        /// <summary>
        /// Function that starts all tasks running in backgrouds
        /// </summary>
        public void StartTasks()
        {
            while (!IsHandleCreated) { Thread.Sleep(10); }
            Task.Run(AskingData);
            Task.Run(LogClients);
        }

        /// <summary>
        /// Function that find the teacher ip
        /// </summary>
        public void FindIp()
        {
            // Establish the local endpointfor the socket.
            // Dns.GetHostName returns the name of the host running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> PossiblesIp = new();
            foreach (IPAddress address in ipHost.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) { PossiblesIp.Add(address); }
            }
            switch (PossiblesIp.Count)
            {
                case 0:
                    MessageBox.Show("Aucune addresse ip conforme n'a étée trouvée.\r\n" +
                        "Vérifiez vos connexion aux réseaux.\r\n" +
                        "L'application va ce fermer.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.Exit();
                    break;
                case 1: ipAddr = PossiblesIp[0]; break;
                default:
                    AskToChoseIp prompt = new(PossiblesIp);
                    if (prompt.ShowDialog(this) == DialogResult.OK) { ipAddr = prompt.GetChoosenIp(); }
                    else { ipAddr = PossiblesIp[0]; }
                    prompt.Dispose();
                    break;
            }
            lblIP.Text = "IP: " + ipAddr.ToString();
        }

        #endregion

        #region New Student

        /// <summary>
        /// Function that connect the students
        /// </summary>
        public void LogClients()
        {
            while (!IsHandleCreated) { Thread.Sleep(100); }
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
                    lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Nouvelle connexion de: " + clientSocket.RemoteEndPoint); }));
                    AllStudents.Add(new DataForTeacher(clientSocket, NextId));
                    Task.Run(() => SendAutorisedUrl(clientSocket));
                    NextId++;
                }
                catch (Exception e) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(e.ToString()); })); }
            }
        }

        /// <summary>
        /// Function that sends the autorised urls to a student
        /// </summary>
        /// <param name="socket"></param>
        public void SendAutorisedUrl(Socket socket)
        {
            while (isAsking) { Thread.Sleep(100); }
            isAsking = true;
            socket.Send(Encoding.Default.GetBytes("url"));
            //serialization
            string jsonString = JsonSerializer.Serialize(Properties.Settings.Default.AutorisedWebsite);
            //envoi
            Thread.Sleep(100);
            socket.Send(Encoding.ASCII.GetBytes(jsonString), Encoding.ASCII.GetBytes(jsonString).Length, SocketFlags.None);
            isAsking = false;
        }

        #endregion

        #region Transfert

        /// <summary>
        /// Function that updates all students
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
                    DateTime NextUpdate = DateTime.Now.AddSeconds(Properties.Settings.Default.TimeBetweenDemand);
                    List<DataForTeacher> ClientToRemove = new();
                    UpdateEleves(ClientToRemove);
                    foreach (DataForTeacher client in ClientToRemove)
                    {
                        RemoveStudent(client);
                    }
                    UpdateAllIndividualDisplay();
                    DateTime FinishedUpdate = DateTime.Now;
                    TimeSpan UpdateDuration = FinishedUpdate - StartUpdate;
                    TimeSpan CycleDuration = NextUpdate - StartUpdate;
                    if (CycleDuration > UpdateDuration)
                    {
                        isAsking = false;
                        lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Attente du prochain cycle dans " + (CycleDuration - UpdateDuration) + " secondes"); }));
                        Thread.Sleep(CycleDuration - UpdateDuration);
                    }
                }
                else { Thread.Sleep(100); }
            }
        }

        /// <summary>
        /// Function that asks a studen for their data and screenshots
        /// </summary>
        /// <param name="ClientToRemove"></param>
        public void UpdateEleves(List<DataForTeacher> ClientToRemove)
        {
            for (int i = 0; i < AllStudents.Count; i++)
            {
                Socket socket = AllStudents[i].SocketToStudent;
                socket.ReceiveTimeout = Properties.Settings.Default.DefaultTimeout;
                socket.SendTimeout = Properties.Settings.Default.DefaultTimeout;
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
        /// Fonction qui permet de recevoir les données envoiées par un élève
        /// </summary>
        /// <param name="student">the student that sent the data</param>
        /// <returns></returns>
        private DataForTeacher ReceiveData(DataForTeacher student)
        {
            try
            {
                Socket socket = student.SocketToStudent;
                int id = student.ID;
                byte[] dataBuffer = new byte[100000];
                socket.ReceiveTimeout = Properties.Settings.Default.DefaultTimeout;
                int nbData = socket.Receive(dataBuffer);
                Array.Resize(ref dataBuffer, nbData);
                Data data = JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer));
                student = new(data)
                {
                    SocketToStudent = socket,
                    ID = id
                };
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Données recue de " + student.UserName); }));
                Task.Run(() => UpdateTreeViews(student));
                student.NumberOfFailure = 0;
                return student;
            }
            catch
            {
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé de donnée"); }));
                student.NumberOfFailure++;
                return student;
            }
        }

        /// <summary>
        /// Function that receives the screenshot sent by the student
        /// </summary>
        /// <param name="student">The student that sent the image</param>
        private void ReceiveImage(DataForTeacher student)
        {
            try
            {
                Socket socket = student.SocketToStudent;
                byte[] imageBuffer = new byte[10485760];
                socket.ReceiveTimeout = Properties.Settings.Default.DefaultTimeout;
                int nbData = socket.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
                Array.Resize(ref imageBuffer, nbData);
                student.ScreenShot = new Bitmap(new MemoryStream(imageBuffer));
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Image recue de " + student.UserName); }));
                student.NumberOfFailure = 0;
                Displayer.UpdateMiniature(student.ID, student.ComputerName, student.ScreenShot);
            }
            catch
            {
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " " + student.UserName + "n'a pas envoyé d'image"); }));
                student.NumberOfFailure++;
            }
        }

        #endregion

        #region TreeView update

        /// <summary>
        /// Function that updates the treeviews
        /// </summary>
        /// <param name="student">the student that is updated</param>
        public void UpdateTreeViews(DataForTeacher student)
        {
            TreeViewDetails.Invoke(new MethodInvoker(delegate
            {
                TreeNode nodeStudent;
                try { nodeStudent = TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false)[0]; }
                catch { nodeStudent = TreeViewDetails.Nodes.Add(Convert.ToString(student.ID), student.UserName + " : " + student.ComputerName); }
                TreeNode nodeProcess;
                try { nodeProcess = nodeStudent.Nodes[0]; }
                catch { nodeProcess = nodeStudent.Nodes.Add("Processus:"); }
                TreeNode nodeNavigateurs;
                try { nodeNavigateurs = nodeStudent.Nodes[1]; }
                catch { nodeNavigateurs = nodeStudent.Nodes.Add("Navigateurs:"); }
                nodeProcess.Nodes.Clear();
                UpdateTreeView.UpdateProcess(student, nodeProcess, null, Configuration.GetFilterEnabled(), Properties.Settings.Default.AlertedProcesses, Properties.Settings.Default.IgnoredProcesses);
                UpdateTreeView.UpdateUrls(student, nodeNavigateurs, null);
                UpdateTreeView.ApplyUrlFilter(nodeNavigateurs, Properties.Settings.Default.AlertedUrls);
            }));
            TreeViewSelect.Invoke(new MethodInvoker(delegate
            {
                TreeNode nodeStudent;
                try { nodeStudent = TreeViewSelect.Nodes.Find(Convert.ToString(student.ID), false)[0]; }
                catch { nodeStudent = TreeViewSelect.Nodes.Add(Convert.ToString(student.ID), student.UserName + " : " + student.ComputerName); }
            }));
        }

        #endregion

        /// <summary>
        /// Function that applys the filters in the treeview
        /// </summary>
        /// <param name="student">L'élève à enlever</param>
        public void RemoveStudent(DataForTeacher student)
        {
            AllStudents.Remove(student);
            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " L'élève " + student.UserName + " est déconnecté"); }));
            TreeViewDetails.Invoke(new MethodInvoker(delegate
            {
                TreeNode[] nodes = TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false);
                if (nodes.Any()) { nodes[0].Remove(); }
            }));
            TreeViewSelect.Invoke(new MethodInvoker(delegate
            {
                TreeNode[] nodes = TreeViewDetails.Nodes.Find(Convert.ToString(student.ID), false);
                if (nodes.Any()) { nodes[0].Remove(); }
            }));
        }

        #region Miniatures

        /// <summary>
        /// Function that creates or remove the screenshots when a checkbox is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeNodeChecked(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) { return; }
            if (e.Node.Checked)
            {
                DataForTeacher student = null;
                foreach (DataForTeacher students in AllStudents)
                {
                    if (Convert.ToString(students.ID) == e.Node.Name) { student = students; }
                }
                if (student == null) { return; }
                foreach (Miniature mini in Displayer.MiniatureList) { if (mini.GetComputerName() == student.ComputerName && mini.StudentID == student.ID) { return; } }
                Miniature miniature = new(student.ScreenShot, student.ComputerName, student.ID, Properties.Settings.Default.PathToSaveFolder);
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
        /// Fonction qui met à jour les miniatures lorsque le panel est redimensionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelMiniatures_Resize(object sender, EventArgs e)
        {
            Displayer.UpdateAllLocations(panelMiniatures.Width);
        }

        #endregion

        #region Stream

        /// <summary>
        /// Function that take screenshots and share them in multicast.
        /// </summary>
        public void RecordAndStreamScreen()
        {
            foreach (DataForTeacher student in StudentToShareScreen) { student.SocketToStudent.Send(Encoding.ASCII.GetBytes("receive")); }

            Socket s = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.Parse("232.1.2.3").GetAddressBytes());
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10);
            IPEndPoint ipep = new(ip, 45678);
            s.Connect(ipep);
            Thread.Sleep(1000);

            while (isSharing)
            {
                Screen screen = Screen.AllScreens[Properties.Settings.Default.ScreenToShareId];
                Bitmap bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format16bppRgb565);
                Rectangle ScreenSize = screen.Bounds;
                Graphics.FromImage(bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
                ImageConverter converter = new();
                byte[] imageArray = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                CustomMessageSender Sender = new(imageArray);
                foreach (CustomMessage message in Sender.GetMessages())
                {
                    s.Send(message.GetContent().ToArray());
                }
            }
        }

        /// <summary>
        /// Function starts or stops the stream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShareScreen(object sender, EventArgs e)
        {
            if (!isSharing)
            {
                ChooseStreamOptions prompt = new(AllStudents);
                if (prompt.ShowDialog(this) != DialogResult.OK) { return; }
                if (StudentToShareScreen.Count == 0) { return; }
                StudentToShareScreen = prompt.GetStudentToShare();
                isSharing = true;
                SendStreamConfiguration();
                ScreenSharer = Task.Run(RecordAndStreamScreen);
                btnShare.Text = "Stop Sharing";
            }
            else
            {
                isSharing = false;
                for (int i = 0; i < StudentToShareScreen.Count; i++)
                { StudentToShareScreen[i].SocketToStudent.Send(Encoding.Default.GetBytes("stops")); }
                StudentToShareScreen = new();
                btnShare.Text = "Share screen";
                ScreenSharer.Wait();
                ScreenSharer.Dispose();
            }
        }

        /// <summary>
        /// Function that send the stream configuration to all relevent students
        /// </summary>
        private void SendStreamConfiguration()
        {
            byte[] bytes = Encoding.Default.GetBytes(JsonSerializer.Serialize(Configuration.GetStreamOptions()));
            foreach (DataForTeacher student in StudentToShareScreen)
            {
                student.SocketToStudent.Send(Encoding.Default.GetBytes("apply"));
            }
            Thread.Sleep(100);
            foreach (DataForTeacher student in StudentToShareScreen)
            {
                student.SocketToStudent.Send(bytes);
            }
        }

        #endregion

        #region Teacher actions

        /// <summary>
        /// Function that shows the trayicon if the application is minimized
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
        /// Function that reopens the application when the trayicon is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconTeacherClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Function that signal to the student the closure of the teacher application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender, FormClosedEventArgs e)
        {
            for (int i = 0; i < AllStudents.Count; i++)
            {
                DataForTeacher student = AllStudents[i];
                try
                {
                    student.SocketToStudent.Send(Encoding.Default.GetBytes("stops"));
                    student.SocketToStudent.Send(Encoding.ASCII.GetBytes("disconnect"));
                }
                catch {/*Student has already closed the application.*/ }
                student.SocketToStudent.Dispose();
            }
            TrayIconTeacher.Visible = false;
            TrayIconTeacher.Dispose();
        }

        /// <summary>
        /// Function that resizes the screenshot when the slider is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_Scroll(object sender, EventArgs e)
        {
            Displayer.Zoom = Slider.Value / 100.0;
            Displayer.ChangeZoom();
        }

        /// <summary>
        /// Function that verifies the node click before opening a new display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //vérification que la node est un ordinateur
            if (e.Node == null) return;
            foreach (DataForTeacher student in AllStudents)
            {
                if (student.ID == Convert.ToInt32(e.Node.Name)) { OpenPrivateDisplay(student); return; }
            }
        }

        #endregion

        #region Individual display

        /// <summary>
        /// Fonction qui met à jour tous les affichage individuels
        /// </summary>
        public void UpdateAllIndividualDisplay()
        {
            foreach (DisplayStudent display in AllStudentsDisplay)
            {
                foreach (DataForTeacher student in AllStudents)
                {
                    if (display.GetStudentId() == student.ID)
                    {
                        display.UpdateAffichage(student);
                    }
                }
            }
        }

        /// <summary>
        /// Function that creates a new individual display
        /// </summary>
        /// <param name="student"></param>
        public void OpenPrivateDisplay(DataForTeacher student)
        {
            foreach (DisplayStudent display in AllStudentsDisplay)
            {
                if (display.GetStudentId() == student.ID) { return; }
            }
            DisplayStudent newDisplay = new(ipAddr);
            AllStudentsDisplay.Add(newDisplay);
            newDisplay.UpdateAffichage(student);
            newDisplay.FormClosing += new FormClosingEventHandler(RemovePrivateDisplay);
            newDisplay.Show();
        }

        /// <summary>
        /// Function that removes the individual display when it is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemovePrivateDisplay(object sender, FormClosingEventArgs e)
        {
            DisplayStudent closingDisplay = (DisplayStudent)sender;
            AllStudentsDisplay.Remove(closingDisplay);
        }

        #endregion

        #region Filter

        /// <summary>
        /// Function that enable or disable the filters in the treeviews
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonFilter_Click(object sender, EventArgs e)
        {
            Configuration.SetFilterEnabled(!Configuration.GetFilterEnabled());
            if (Configuration.GetFilterEnabled())
            {
                btnFilter.Text = "Désactiver";
                foreach (DataForTeacher student in AllStudents)
                {
                    UpdateTreeViews(student);
                }
            }
            else
            {
                btnFilter.Text = "Activer";
                foreach (TreeNode node in TreeViewDetails.Nodes)
                {
                    RemoveFilter(node);
                }
            }
        }

        /// <summary>
        /// Function that removes the background color in all nodes
        /// </summary>
        /// <param name="node"></param>
        void RemoveFilter(TreeNode node)
        {
            node.BackColor = Color.White;
            foreach (TreeNode subnode in node.Nodes)
            {
                RemoveFilter(subnode);
            }
        }

        #endregion

        #region TreeView display

        /// <summary>
        /// Function that closes all treenode in the treeviews
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HideTreeView_Click(object sender, EventArgs e)
        {
            TreeNodeCollection nodes = TreeViewDetails.Nodes;
            foreach (TreeNode node in nodes)
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
        /// Function that opens all treenode in the treeviews
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

        #endregion

        /// <summary>
        /// Function that open the configuration window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenConfigWindow_Click(object sender, EventArgs e)
        {
            ConfigurationWindow configWindow = new();
            configWindow.Show();
        }
    }
}
