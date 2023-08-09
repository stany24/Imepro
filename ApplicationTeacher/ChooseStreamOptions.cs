﻿using LibraryData;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class ChooseStreamOptions : Form
    {
        readonly ConfigurationDynamique config = new();
        public ChooseStreamOptions(List<DataForTeacher> list)
        {
            InitializeComponent();
            lbxStudents.SelectionMode = SelectionMode.MultiExtended;
            lbxStudents.DataSource = list;
            foreach(KeyValuePair<string,List<string>> focus in config.AllFocus) { lbxFocus.Items.Add(focus); }
            lbxFocus.DisplayMember= "Key";
            lbxPriorite.DataSource = Enum.GetNames(typeof(Priority));
            lbxScreen.DataSource = Screen.AllScreens;
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

        private void BeginSharing_Click(object sender, EventArgs e)
        {
            if(lbxStudents.SelectedItems.Count == 0) { lblError.Text = "Selectionnez au moins 1 élève"; return; }
            if(lbxPriorite.SelectedItem == null) { lblError.Text = "Selectionnez la priorité"; return; }
            if(lbxFocus.SelectedItem == null) { lblError.Text = "Selectionnez le focus"; return; }
            ConfigurationStatic.StudentToShareScreen.Clear();
            foreach(DataForTeacher student in lbxStudents.SelectedItems)
            {
                ConfigurationStatic.StudentToShareScreen.Add(student);
            }
            List<string> focus = (List<string>)lbxFocus.SelectedItem;
            Priority priorite = (Priority)Enum.Parse(typeof(Priority), lbxPriorite.SelectedItem.ToString());
            Properties.Settings.Default.ScreenToShareId = lbxScreen.SelectedIndex;
            ConfigurationStatic.streamoptions = new StreamOptions(priorite, focus);
        }
    }
}
