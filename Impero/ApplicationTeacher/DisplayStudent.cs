using LibraryData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class DisplayStudent : Form
    {
        public int StudentId;
        public DisplayStudent()
        {
            InitializeComponent();
        }

        public void UpdateAffichage(DataForTeacher student)
        {
            if (InvokeRequired)
            {
                lblPoste.Invoke(new MethodInvoker(delegate { lblPoste.Text = "Poste: " + student.ComputerName; }));
                lblUserName.Invoke(new MethodInvoker(delegate { lblUserName.Text = "Nom: " + student.UserName; }));
                lbxProcesses.Invoke(new MethodInvoker(delegate {
                    lbxProcesses.Items.Clear();
                    foreach (KeyValuePair<int, string> process in student.Processes) { lbxProcesses.Items.Add(process.Value); }
                }));
                lbxUrls.Invoke(new MethodInvoker(delegate {
                    lbxUrls.Items.Clear();
                    //foreach (String url in student.Urls) { lbxUrls.Items.Add(url); }
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
                lbxUrls.Items.Clear();
                //foreach (String url in student.Urls) { lbxUrls.Items.Add(url); }
                pbxScreenShot.Image = student.ScreenShot;
            }
        }
    }
}
