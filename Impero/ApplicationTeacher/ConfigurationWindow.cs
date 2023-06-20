using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class ConfigurationWindow : Form
    {
        ConfigurationDynamique config;
        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        private void ConfigurationWindow_Load(object sender, EventArgs e)
        {
            config = new ConfigurationDynamique();
            foreach(KeyValuePair<string,List<string>> list in config.AllLists)
            {
                cbxLists.Items.Add(list.Key);
            }
        }

        private void cbxLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbxStrings.Items.Clear();
            foreach(string str in config.AllLists[cbxLists.SelectedItem.ToString()])
            {
                lbxStrings.Items.Add(str);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach(string item in lbxStrings.SelectedItems)
            {
                config.AllLists[cbxLists.SelectedItem.ToString()].Remove(item);
            }
            while (lbxStrings.SelectedItems.Count > 0)
            {
                lbxStrings.Items.Remove(lbxStrings.SelectedItems[0]);
            }
        }

        private void btnAddString_Click(object sender, EventArgs e)
        {
            config.AllLists[cbxLists.SelectedItem.ToString()].Add(tbxAddString.Text);
            lbxStrings.Items.Add(tbxAddString.Text);
            tbxAddString.Text = string.Empty;
        }
    }
}
