using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class ConfigurationWindow : Form
    {
        ConfigurationDynamique config;
        public ConfigurationWindow()
        {
            InitializeComponent();
            nudTimeBetweenAsking.Value = Properties.Settings.Default.TimeBetweenDemand;
            tbxSaveFolder.Text = Properties.Settings.Default.PathToSaveFolder;
        }

        private void ConfigurationWindow_Load(object sender, EventArgs e)
        {
            config = new ConfigurationDynamique();
            cbxCategory.Items.Add("Listes");
            cbxCategory.Items.Add("Focus");
        }

        private void CategoryChanged(object sender, EventArgs e)
        {
            cbxParameter.Items.Clear();
            lbxStrings.Items.Clear();
            switch (cbxCategory.SelectedItem.ToString())
            {
                case "Listes":
                    foreach (KeyValuePair<string, List<string>> list in config.AllLists)
                    {cbxParameter.Items.Add(list.Key);}
                    break;
                case "Focus":
                    foreach (KeyValuePair<string, List<string>> list in config.AllFocus)
                    {cbxParameter.Items.Add(list.Key);};
                    break;
            }
        }
        private void ParameterChanged(object sender, EventArgs e)
        {
            lbxStrings.Items.Clear();
            switch (cbxCategory.SelectedItem.ToString())
            {
                case "Listes":
                    foreach (string str in config.AllLists[cbxParameter.SelectedItem.ToString()])
                    {lbxStrings.Items.Add(str);}
                    ; break;
                case "Focus":
                    foreach (string str in config.AllFocus[cbxParameter.SelectedItem.ToString()])
                    {lbxStrings.Items.Add(str);}
                    break;
            }
        }

        private void RemoveSelectedString_Click(object sender, EventArgs e)
        {
            switch (cbxCategory.SelectedItem.ToString())
            {
                case "Listes":
                    foreach (string item in lbxStrings.SelectedItems)
                    { config.AllLists[cbxParameter.SelectedItem.ToString()].Remove(item);}
                    break;
                case "Focus":
                    foreach (string item in lbxStrings.SelectedItems)
                    { config.AllFocus[cbxParameter.SelectedItem.ToString()].Remove(item);}
                    break;
            }
            while (lbxStrings.SelectedItems.Count > 0)
            {lbxStrings.Items.Remove(lbxStrings.SelectedItems[0]);}
        }

        private void AddString_Click(object sender, EventArgs e)
        {
            switch (cbxCategory.SelectedItem.ToString())
            {
                case "Listes":
                    config.AllLists[cbxParameter.SelectedItem.ToString()].Add(tbxAddString.Text);
                    break;
                case "Focus":
                    config.AllFocus[cbxParameter.SelectedItem.ToString()].Add(tbxAddString.Text);
                    break;
            }
            lbxStrings.Items.Add(tbxAddString.Text);
            tbxAddString.Text = string.Empty;
        }

        private void ApplyChanges(object sender, EventArgs e)
        {
            Properties.Settings.Default.TimeBetweenDemand = (int)nudTimeBetweenAsking.Value;
            Properties.Settings.Default.Save();
        }

        private void ChangeSaveFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = fbdSaveFolder.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbdSaveFolder.SelectedPath))
            {
                tbxSaveFolder.Text = fbdSaveFolder.SelectedPath;
                Properties.Settings.Default.PathToSaveFolder = fbdSaveFolder.SelectedPath;
            }
        }
    }
}
