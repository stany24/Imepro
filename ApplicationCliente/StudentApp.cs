using LibraryData;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApplicationCliente
{
    public partial class StudentApp : Form
    {

        #region Variables

        readonly DataForStudent Student;

        #endregion

        #region At start
        public StudentApp()
        {
            InitializeComponent();
            Student = new(lbxConnexion, lbxMessages, this);
            Student.ChangePropertyEvent += ChangeProperty;
            try
            {
                Student.IpToTeacher = IpForTheWeek.GetIp();
            }
            catch (Exception)
            {
                NewTeacherIP(new object(), new EventArgs());
            }
            Task.Run(LaunchTasks);
        }

        private void ChangeProperty(object sender,ChangePropertyEventArgs e)
        {
            Control control = Controls.Find(e.PropertyName, true)[0];
            PropertyInfo propInfo = control.GetType().GetProperty(e.PropertyName);
            if (propInfo.CanWrite)
            {
                propInfo.SetValue(control, e.PropertyValue);
            }
        }

        /// <summary>
        /// Function waiting for the form to be fully created before launching background tasks.
        /// </summary>
        public void LaunchTasks()
        {
            while (!IsHandleCreated) { Thread.Sleep(100); }
            Student.SocketToTeacher = Task.Run(() => Student.ConnectToTeacher(11111)).Result;
        }

        #endregion

        #region teacher ip

        /// <summary>
        /// Function that asks to the student the new teacher ip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NewTeacherIP(object sender, EventArgs e)
        {
            AskIp prompt = new();
            prompt.ShowDialog();
            prompt.Close();
            prompt.Dispose();
            Student.IpToTeacher = IpForTheWeek.GetIp();
        }

        /// <summary>
        /// Function that verify the interfaces, if they are incorrect it returns a script.
        /// </summary>
        /// <returns> The command to excecute in powershell</returns>
        public string CheckInterfaces()
        {
            NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();
            StringBuilder commandBuilder = new();
            foreach (NetworkInterface current in netInterface)
            {
                bool isCorrect = false;
                IPInterfaceProperties properities = current.GetIPProperties();
                foreach (UnicastIPAddressInformation ip in properities.UnicastAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    if (IsOnSameNetwork(Student.IpToTeacher, ip.Address, ip.IPv4Mask)) { isCorrect = true; }
                }

                commandBuilder.Append("netsh interface ipv4 set interface \"");
                commandBuilder.Append(current.Name);
                if (!isCorrect) { commandBuilder.Append("\" forwarding=disable\r\n"); }
                else { commandBuilder.Append("\" forwarding=enable\r\n"); }
            }
            return commandBuilder.ToString();
        }

        /// <summary>
        /// Function to know if two adresses are on the same network.
        /// </summary>
        /// <param name="ipAddress1">The first ip.</param>
        /// <param name="ipAddress2">The second ip.</param>
        /// <param name="subnetMask">The adress mask.</param>
        /// <returns>If the addresses are on the same network.</returns>
        public static bool IsOnSameNetwork(IPAddress ipAddress1, IPAddress ipAddress2, IPAddress subnetMask)
        {
            byte[] ipBytes1 = ipAddress1.GetAddressBytes();
            byte[] ipBytes2 = ipAddress2.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            for (int i = 0; i < ipBytes1.Length; i++)
            {
                if ((ipBytes1[i] & subnetMaskBytes[i]) != (ipBytes2[i] & subnetMaskBytes[i])) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Function to help the user configure its interfaces.
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

        #endregion

        #region interaction

        /// <summary>
        /// Function that communicate to the teacher the closure of the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender, FormClosedEventArgs e)
        {
            if (Student.SocketToTeacher == null) { return; }
            try
            {
                Student.SocketToTeacher.Send(Encoding.ASCII.GetBytes("stop"));
                Student.SocketToTeacher.Disconnect(false);
                Student.SocketToTeacher = null;
            }
            catch
            {
                // Should not happend
            }
        }

        /// <summary>
        /// Function that displays the trayicon if the application is minimized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StudentAppResized(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Minimized:
                    TrayIconStudent.Visible = true; Hide();
                    break;
                case FormWindowState.Normal:
                    TrayIconStudent.Visible = false;
                    break;
                case FormWindowState.Maximized:
                    TrayIconStudent.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// Function to show the application when the trayicon is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TrayIconStudentClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        #endregion

        private void WebView2_Click(object sender, EventArgs e)
        {
            Form form = new();
            Browser browser = new();
            browser.NewTabEvent += new EventHandler<NewTabEventArgs>(AddWebview2Url);
            form.Controls.Add(browser);
            form.Show();
        }

        private void AddWebview2Url(object sender, NewTabEventArgs e)
        {
            Student.Urls.AddUrl(e.url, BrowserName.Webview2);
        }
    }
}
