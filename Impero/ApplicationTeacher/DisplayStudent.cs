using LibraryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace ApplicationTeacher
{
    public partial class DisplayStudent : Form
    {
        public DataForTeacher Student = null;
        public IPAddress ipAddr = null;
        PictureBox pbxStream;
        public DisplayStudent(IPAddress ip)
        {
            ipAddr= ip;
            InitializeComponent();
        }

        /// <summary>
        /// Fonction qui met à jour l'affichage individuel
        /// </summary>
        /// <param name="student">Les nouvelles données</param>
        public void UpdateAffichage(DataForTeacher student)
        {
            if(Student != null && Student.SocketControl != null) { student.SocketControl = Student.SocketControl; }
            Student= student;
            if (InvokeRequired)
            {
                lblPoste.Invoke(new MethodInvoker(delegate { lblPoste.Text = "Poste: " + student.ComputerName; }));
                lblUserName.Invoke(new MethodInvoker(delegate { lblUserName.Text = "Nom: " + student.UserName; }));
                TreeViewProcesses.Invoke(new MethodInvoker(delegate {
                    foreach (KeyValuePair<int, string> process in student.Processes) {
                        TreeNode current = TreeViewProcesses.Nodes.Add(Convert.ToString(process.Key), process.Value);
                        if (Properties.Settings.Default.AlertedProcesses.Find(x => x == process.Value) != null)
                        {
                            current.BackColor = Color.Red;
                            while (current.Parent != null)
                            {
                                current = current.Parent;
                                current.BackColor = Color.Red;
                            }
                        }
                    }
                }));

                TreeViewUrls.Invoke(new MethodInvoker(delegate {
                    UpdateUrlsTree(student.Urls.AllBrowser["chrome"] , "chrome","Chrome");
                    UpdateUrlsTree(student.Urls.AllBrowser["firefox"], "firefox","Firefox");
                    UpdateUrlsTree(student.Urls.AllBrowser["msedge"], "msedge","Edge");
                    UpdateUrlsTree(student.Urls.AllBrowser["opera"], "opera","Opera");
                    UpdateUrlsTree(student.Urls.AllBrowser["iexplorer"], "iexplorer","Internet Explorer");
                    UpdateUrlsTree(student.Urls.AllBrowser["safari"], "safari","Safari");
                }));
                pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = student.ScreenShot; }));
            }
            else
            {
                Text = student.UserName;
                lblPoste.Text = "Poste: " + student.ComputerName;
                lblUserName.Text = "Nom: " + student.UserName;
                foreach (KeyValuePair<int, string> process in student.Processes)
                {
                    TreeNode current = TreeViewProcesses.Nodes.Add(Convert.ToString(process.Key), process.Value);
                    if (Properties.Settings.Default.AlertedProcesses.Find(x => x == process.Value) != null)
                    {
                        current.BackColor = Color.Red;
                        while (current.Parent != null)
                        {
                            current = current.Parent;
                            current.BackColor = Color.Red;
                        }
                    }
                }
                UpdateUrlsTree(student.Urls.AllBrowser["chrome"], "chrome", "Chrome");
                UpdateUrlsTree(student.Urls.AllBrowser["firefox"], "firefox", "Firefox");
                UpdateUrlsTree(student.Urls.AllBrowser["msedge"], "msedge", "Edge");
                UpdateUrlsTree(student.Urls.AllBrowser["opera"], "opera", "Opera");
                UpdateUrlsTree(student.Urls.AllBrowser["iexplorer"], "iexplorer", "Internet Explorer");
                UpdateUrlsTree(student.Urls.AllBrowser["safari"], "safari", "Safari");
                pbxScreenShot.Image = student.ScreenShot;
            }
        }

        public void UpdateUrlsTree(List<Url> urls, string ProcessName, string DisplayName)
        {
            if (InvokeRequired)
            {
                TreeViewUrls.Invoke(new MethodInvoker(delegate {
                    if (urls.Count == 0) { try { TreeViewUrls.Nodes.Find(ProcessName, false)[0].Remove(); } catch { }; return; }

                    TreeNode NodeNavigateur;
                    try{NodeNavigateur = TreeViewUrls.Nodes.Find(ProcessName, false)[0];}
                    catch{NodeNavigateur = TreeViewUrls.Nodes.Add(ProcessName, DisplayName);}

                    bool isAlerted = false;
                        for (int i = NodeNavigateur.Nodes.Count; i < urls.Count; i++)
                        {
                            TreeNode NodeUrl = NodeNavigateur.Nodes.Add(urls[i].ToString());
                            for (int j = 0; j < Properties.Settings.Default.AlertedUrls.Count; j++)
                            {
                                if (urls[i].Name.ToLower().Contains(Properties.Settings.Default.AlertedUrls[j]))
                                {
                                    NodeUrl.BackColor = Color.Red;
                                    NodeNavigateur.BackColor = Color.Red;
                                    isAlerted = true;
                                }
                            }
                            if (isAlerted == false)
                            {
                                NodeUrl.BackColor = Color.White;
                                NodeNavigateur.BackColor = Color.White;
                            }
                        }
                    return;
                }));
            }
            else
            {
                if (urls.Count == 0) { try { TreeViewUrls.Nodes.Find(ProcessName, false)[0].Remove(); } catch { }; return; }

                TreeNode NodeNavigateur;
                try { NodeNavigateur = TreeViewUrls.Nodes.Find(ProcessName, false)[0]; }
                catch { NodeNavigateur = TreeViewUrls.Nodes.Add(ProcessName, DisplayName); }

                bool isAlerted = false;
                for (int i = NodeNavigateur.Nodes.Count; i < urls.Count; i++)
                {
                    TreeNode NodeUrl = NodeNavigateur.Nodes.Add(urls[i].ToString());
                    for (int j = 0; j < Properties.Settings.Default.AlertedUrls.Count; j++)
                    {
                        if (urls[i].Name.ToLower().Contains(Properties.Settings.Default.AlertedUrls[j]))
                        {
                            NodeUrl.BackColor = Color.Red;
                            NodeNavigateur.BackColor = Color.Red;
                            isAlerted = true;
                        }
                    }
                    if (isAlerted == false)
                    {
                        NodeUrl.BackColor = Color.White;
                        NodeNavigateur.BackColor = Color.White;
                    }
                }
                return;
            }
        }

        /// <summary>
        /// Fonction qui permet de sauvegarder le screenshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveScreenShot(object sender, EventArgs e)
        {
            pbxScreenShot.Image.Save(Properties.Settings.Default.PathToSaveFolder +Student.ComputerName+ DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        /// <summary>
        /// Fonction qui permet d'envoyer un message à l'élève
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessage(object sender, EventArgs e)
        {
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("message"));
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes(tbxMessage.Text));
        }

        /// <summary>
        /// Fonction qui permet de prendre le controle de l'élève
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TakeControl_Click(object sender, EventArgs e)
        {
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("control 0"));
            ConnectStudentForControl();
            pbxStream = new(){
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            Controls.Add(pbxStream);
            Controls.SetChildIndex(pbxStream, 0);
            Task.Run(ReceiveStream);
        }

        /// <summary>
        /// Fonction qui permet de recevoir le stream de l'élève
        /// </summary>
        private void ReceiveStream()
        {
            while (true)
            {
                try
                {
                    byte[] imageBuffer = new byte[10485760];
                    int nbData = Student.SocketControl.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
                    Array.Resize(ref imageBuffer, nbData);
                    pbxStream.Image = new Bitmap(new MemoryStream(imageBuffer));
                }
                catch {}
            }
        }

        /// <summary>
        /// Fonction qui permet de connecter les élèves qui en font la demande
        /// </summary>
        public void ConnectStudentForControl()
        {
            while (IsHandleCreated == false) { Thread.Sleep(100); }
            IPEndPoint localEndPoint = new(ipAddr, 11112);
            // Creation TCP/IP Socket using Socket Class Constructor
            Socket listener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Using Bind() method we associate a network address to the Server Socket
            // All client that will connect to this Server Socket must know this network Address
            listener.Bind(localEndPoint);
            // Using Listen() method we create the Client list that will want to connect to Server
            listener.Listen(1);
            while (Student.SocketControl == null)
            {
                try
                {
                    // Suspend while waiting for incoming connection Using Accept() method the server will accept connection of client
                    Student.SocketControl = listener.Accept();
                    Student.SocketControl.ReceiveTimeout = Properties.Settings.Default.TimeBetweenDemand;
                    return;
                }
                catch{}
            }
        }

        /// <summary>
        /// Fonction qui permet d'arreter une application élève
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShutDown_Click(object sender, EventArgs e)
        {
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("shutdown"));
            Student.SocketToStudent.Disconnect(false);
            this.Close();
        }

        private void KillProcess_Click(object sender, EventArgs e)
        {
            if (TreeViewProcesses.SelectedNode == null) { return; }
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("kill "+TreeViewProcesses.SelectedNode.Name));
            TreeViewProcesses.SelectedNode.Remove();
        }
    }
}
