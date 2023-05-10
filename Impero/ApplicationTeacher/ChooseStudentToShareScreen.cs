using LibraryData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{   
    public partial class ChooseStudentToShareScreen : Form
    {
        public ChooseStudentToShareScreen(List<DataForTeacher> list)
        {
            InitializeComponent();
            lbxStudents.SelectionMode = SelectionMode.MultiExtended;
            lbxStudents.DataSource = list;
            lbxFocus.DataSource = Enum.GetNames(typeof(Focus));
            lbxPriorite.DataSource = Enum.GetNames(typeof(Priority));
        }

        private void SelectAll(object sender, EventArgs e)
        {
            for (int i = 0; i < lbxStudents.Items.Count; i++)
            {
                lbxStudents.SetSelected(i, true);
            }
        }

        private void SelectNone(object sender, EventArgs e)
        {
            lbxStudents.SelectedItems.Clear();
        }

        private void btnBeginSharing_Click(object sender, EventArgs e)
        {
            if(lbxStudents.SelectedItems.Count == 0) { lblError.Text = "Selectionnez au moins 1 élève"; return; }
            if(lbxPriorite.SelectedItem == null) { lblError.Text = "Selectionnez la priorité"; return; }
            if(lbxFocus.SelectedItem == null) { lblError.Text = "Selectionnez le focus"; return; }
            Configuration.StudentToShareScreen.Clear();
            foreach(DataForTeacher student in lbxStudents.SelectedItems)
            {
                Configuration.StudentToShareScreen.Add(student);
            }
            Focus focus = (Focus)Enum.Parse(typeof(Focus), lbxFocus.SelectedItem.ToString());
            Priority priorite = (Priority)Enum.Parse(typeof(Priority), lbxPriorite.SelectedItem.ToString());
            Configuration.streamoptions = new StreamOptions(priorite, focus);
        }
    }
}
