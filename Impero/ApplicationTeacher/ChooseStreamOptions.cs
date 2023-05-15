using LibraryData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{   
    public partial class ChooseStreamOptions : Form
    {
        public ChooseStreamOptions(List<DataForTeacher> list)
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
            List<string> list = new();
            switch(focus)
            {
                case LibraryData.Focus.VSCode:list = new() { "Code" };
                    break;
                case LibraryData.Focus.Word:list = new() { "WINWORD","sppsvc" };
                    break;
                case LibraryData.Focus.VisualStudio:list = new() { "devenv","ServiceHub","" };
                    break;
                case LibraryData.Focus.OneNote:list = new() { "onenoteim" };
                    break;
            }
            Configuration.streamoptions = new StreamOptions(priorite, focus,list);
        }
    }
}
