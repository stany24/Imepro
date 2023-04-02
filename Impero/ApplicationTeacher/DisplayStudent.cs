using LibraryData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class DisplayStudent : Form
    {
        DataForTeacher Student;
        public DisplayStudent(ref DataForTeacher student)
        {
            InitializeComponent();
            Student = student;
        }

        public void UpdateAffichage()
        {
            Text = Student.UserName;
            if (InvokeRequired)
            {
                lblPoste.Invoke(new MethodInvoker(delegate { lblPoste.Text = "Poste: " + Student.ComputerName; }));
                lblUserName.Invoke(new MethodInvoker(delegate { lblUserName.Text = "Nom: " + Student.UserName; }));
                lbxProcesses.Invoke(new MethodInvoker(delegate {
                    lbxProcesses.Items.Clear();
                    foreach (KeyValuePair<int, string> process in Student.Processes) { lbxProcesses.Items.Add(process.Value); }
                }));
                lbxUrls.Invoke(new MethodInvoker(delegate {
                    lbxUrls.Items.Clear();
                    foreach (String url in Student.Urls) { lbxUrls.Items.Add(url); }
                }));
                pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = Student.ScreenShot; }));
            }
            else
            {
                lblPoste.Text = "Poste: " + Student.ComputerName;
                lblUserName.Text = "Nom: " + Student.UserName;
                lbxProcesses.Items.Clear();
                foreach (KeyValuePair<int, string> process in Student.Processes) { lbxProcesses.Items.Add(process.Value); }
                lbxUrls.Items.Clear();
                foreach (String url in Student.Urls) { lbxUrls.Items.Add(url); }
                pbxScreenShot.Image = Student.ScreenShot;
            }
        }
    }
}
