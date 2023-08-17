using LibraryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class DisplayStudent : Form
    {
        #region Variables

        private DataForTeacher Student = null;
        private readonly IPAddress ipAddr = null;
        PictureBox pbxStream;

        #endregion

        public int GetStudentId()
        {
            return Student.ID;
        }

        public DisplayStudent(IPAddress ip)
        {
            ipAddr= ip;
            InitializeComponent();
        }

        #region Display update

        /// <summary>
        /// Function that updates the display with the new data
        /// </summary>
        /// <param name="student">Les nouvelles données</param>
        public void UpdateAffichage(DataForTeacher student)
        {
            if(Student != null && Student.SocketControl != null) { student.SocketControl = Student.SocketControl; }
            Student= student;
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate { this.Text = student.UserName; }));
                lblPoste.Invoke(new MethodInvoker(delegate { lblPoste.Text = "Poste: " + student.ComputerName; }));
                lblUserName.Invoke(new MethodInvoker(delegate { lblUserName.Text = "Nom: " + student.UserName; }));
                TreeViewProcesses.Invoke(new MethodInvoker(delegate {
                    TreeViewProcesses.Nodes.Clear();
                    UpdateTreeView.UpdateProcess(student, null, TreeViewProcesses, Configuration.GetFilterEnabled(), Properties.Settings.Default.AlertedProcesses, Properties.Settings.Default.IgnoredProcesses); }));
                TreeViewUrls.Invoke(new MethodInvoker(delegate {
                    UpdateTreeView.UpdateUrls(student, null, TreeViewUrls);
                }));

            }
            else
            {
                Text = student.UserName;
                lblPoste.Text = "Poste: " + student.ComputerName;
                lblUserName.Text = "Nom: " + student.UserName;
                UpdateTreeView.UpdateProcess(student,null, TreeViewProcesses, Configuration.GetFilterEnabled(), Properties.Settings.Default.AlertedProcesses, Properties.Settings.Default.IgnoredProcesses);
                UpdateTreeView.UpdateUrls(student,null ,TreeViewUrls);
            }
        }

        #endregion

        #region Teacher action

        /// <summary>
        /// Function that saves the current screenshot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveScreenShot(object sender, EventArgs e)
        {
            pbxScreenShot.Image.Save(Properties.Settings.Default.PathToSaveFolder +Student.ComputerName+ DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        /// <summary>
        /// Function that sends a message to the student
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendMessage(object sender, EventArgs e)
        {
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("message"));
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes(tbxMessage.Text));
        }

        /// <summary>
        /// Function that allow the teacher to take controle of the student computer
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

        /// <summary>
        /// Fonction qui arrete le processus séléctionné chez l'élève
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KillProcess_Click(object sender, EventArgs e)
        {
            if (TreeViewProcesses.SelectedNode == null) { return; }
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("kill " + TreeViewProcesses.SelectedNode.Name));
            TreeViewProcesses.SelectedNode.Remove();
        }

        #endregion

        #region Remote control

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
                catch {
                    //lost an image it can happend
                }
            }
        }

        /// <summary>
        /// Function to connect the student to take control
        /// </summary>
        public void ConnectStudentForControl()
        {
            while (!IsHandleCreated) { Thread.Sleep(100); }
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
                catch{
                    listener.Close();
                    Student.SocketControl = null;
                }
            }
        }

        #endregion
    }
}
