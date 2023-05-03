using LibraryData;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class DisplayStudent : Form
    {
        public DataForTeacher Student;
        public string SavePath;
        public DisplayStudent(string savepath)
        {
            SavePath = savepath;
            InitializeComponent();
        }

        public void UpdateAffichage(DataForTeacher student)
        {
            Student= student;
            if (InvokeRequired)
            {
                lblPoste.Invoke(new MethodInvoker(delegate { lblPoste.Text = "Poste: " + student.ComputerName; }));
                lblUserName.Invoke(new MethodInvoker(delegate { lblUserName.Text = "Nom: " + student.UserName; }));
                lbxProcesses.Invoke(new MethodInvoker(delegate {
                    lbxProcesses.Items.Clear();
                    foreach (KeyValuePair<int, string> process in student.Processes) { lbxProcesses.Items.Add(process.Value); }
                }));
                TreeViewUrls.Invoke(new MethodInvoker(delegate {
                    UpdateUrlsTree(TreeViewUrls, student.Urls.chrome, "chrome");
                    UpdateUrlsTree(TreeViewUrls, student.Urls.firefox, "firefox");
                    UpdateUrlsTree(TreeViewUrls, student.Urls.edge, "msedge");
                    UpdateUrlsTree(TreeViewUrls, student.Urls.opera, "opera");
                    UpdateUrlsTree(TreeViewUrls, student.Urls.iexplorer, "iexplorer");
                    UpdateUrlsTree(TreeViewUrls, student.Urls.safari, "safari");
                }));
                pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = student.ScreenShot; }));
            }
            else
            {
                Text = student.UserName;
                lblPoste.Text = "Poste: " + student.ComputerName;
                lblUserName.Text = "Nom: " + student.UserName;
                lbxProcesses.Items.Clear();
                foreach (KeyValuePair<int, string> process in student.Processes) { lbxProcesses.Items.Add(process.Value); }
                UpdateUrlsTree(TreeViewUrls, student.Urls.chrome, "chrome");
                UpdateUrlsTree(TreeViewUrls, student.Urls.firefox, "firefox");
                UpdateUrlsTree(TreeViewUrls, student.Urls.edge, "msedge");
                UpdateUrlsTree(TreeViewUrls, student.Urls.opera, "opera");
                UpdateUrlsTree(TreeViewUrls, student.Urls.iexplorer, "iexplorer");
                UpdateUrlsTree(TreeViewUrls, student.Urls.safari, "safari");
                pbxScreenShot.Image = student.ScreenShot;
            }
        }

        public void UpdateUrlsTree(TreeView NodeAllNavigateur, List<Url> urls, string navigateurName)
        {
            if (urls.Count == 0) { try { NodeAllNavigateur.Nodes.Find(navigateurName, false)[0].Remove(); } catch { }; return; }
            if (InvokeRequired)
            {
                TreeViewUrls.Invoke(new MethodInvoker(delegate {
                    TreeNode[] nodeNavigateur = NodeAllNavigateur.Nodes.Find(navigateurName, false);
                    if (nodeNavigateur.Count() == 0) { NodeAllNavigateur.Nodes.Add(navigateurName, navigateurName); }
                    for (int i = NodeAllNavigateur.Nodes.Find(navigateurName, false)[0].Nodes.Count; i < urls.Count; i++) { NodeAllNavigateur.Nodes.Find(navigateurName, false)[0].Nodes.Add(urls[i].ToString()); }
                }));
            }
            else
            {
                TreeNode[] nodeNavigateur = NodeAllNavigateur.Nodes.Find(navigateurName, false);
                if (nodeNavigateur.Count() == 0) { NodeAllNavigateur.Nodes.Add(navigateurName, navigateurName); }
                for (int i = NodeAllNavigateur.Nodes.Find(navigateurName, false)[0].Nodes.Count; i < urls.Count; i++) { NodeAllNavigateur.Nodes.Find(navigateurName, false)[0].Nodes.Add(urls[i].ToString()); }
            }
        }

        public void SaveScreenShot(object sender, EventArgs e)
        {
            pbxScreenShot.Image.Save(SavePath +Student.ComputerName+ DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        private void SendMessage(object sender, EventArgs e)
        {
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("message"));
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes(tbxMessage.Text));
        }
    }
}
