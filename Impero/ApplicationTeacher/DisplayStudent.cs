using LibraryData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class DisplayStudent : Form
    {
        public DataForTeacher Student;
        public DisplayStudent()
        {
            InitializeComponent();
        }

        public void UpdateAffichage(DataForTeacher student)
        {
            Student= student;
            if (InvokeRequired)
            {
                lblPoste.Invoke(new MethodInvoker(delegate { lblPoste.Text = "Poste: " + student.ComputerName; }));
                lblUserName.Invoke(new MethodInvoker(delegate { lblUserName.Text = "Nom: " + student.UserName; }));
                TreeViewProcesses.Invoke(new MethodInvoker(delegate {
                    foreach (KeyValuePair<int, string> process in student.Processes) {
                        TreeNode current = TreeViewProcesses.Nodes.Add(process.Value);
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
                }));

                TreeViewUrls.Invoke(new MethodInvoker(delegate {
                    UpdateUrlsTree(student.Urls.chrome, "chrome","Chrome");
                    UpdateUrlsTree(student.Urls.firefox, "firefox","Firefox");
                    UpdateUrlsTree(student.Urls.edge, "msedge","Edge");
                    UpdateUrlsTree(student.Urls.opera, "opera","Opera");
                    UpdateUrlsTree(student.Urls.iexplorer, "iexplorer","Internet Explorer");
                    UpdateUrlsTree(student.Urls.safari, "safari","Safari");
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
                    TreeNode current = TreeViewProcesses.Nodes.Add(process.Value);
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
                UpdateUrlsTree(student.Urls.chrome, "chrome", "Chrome");
                UpdateUrlsTree(student.Urls.firefox, "firefox", "Firefox");
                UpdateUrlsTree(student.Urls.edge, "msedge", "Edge");
                UpdateUrlsTree(student.Urls.opera, "opera", "Opera");
                UpdateUrlsTree(student.Urls.iexplorer, "iexplorer", "Internet Explorer");
                UpdateUrlsTree(student.Urls.safari, "safari", "Safari");
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
                            for (int j = 0; j < Configuration.AlertedUrls.Count; j++)
                            {
                                if (urls[i].Name.ToLower().Contains(Configuration.AlertedUrls[j]))
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
                    for (int j = 0; j < Configuration.AlertedUrls.Count; j++)
                    {
                        if (urls[i].Name.ToLower().Contains(Configuration.AlertedUrls[j]))
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

        public void SaveScreenShot(object sender, EventArgs e)
        {
            pbxScreenShot.Image.Save(Configuration.pathToSaveFolder +Student.ComputerName+ DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        private void SendMessage(object sender, EventArgs e)
        {
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes("message"));
            Student.SocketToStudent.Send(Encoding.ASCII.GetBytes(tbxMessage.Text));
        }
    }
}
