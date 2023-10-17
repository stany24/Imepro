using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class ConfigurationWindow : Form
    {
        #region Variable

        private const string IgnoredUrls = "IgnoredUrls";
        private const string AlertedUrls = "AlertedUrls";
        private const string IgnoredProcesses = "IgnoredProcesses";
        private const string AlertedProcesses = "AlertedProcesses";
        private const string AutorisedUrls = "AutorisedUrls";

        #endregion

        #region Constructor

        public ConfigurationWindow()
        {
            InitializeComponent();
            cbxSelectList.Items.Add(IgnoredUrls);
            cbxSelectList.Items.Add(AlertedUrls);
            cbxSelectList.Items.Add(IgnoredProcesses);
            cbxSelectList.Items.Add(AlertedProcesses);
            cbxSelectList.Items.Add(AutorisedUrls);
            nudTimeBetweenAsking.Value = Properties.Settings.Default.TimeBetweenDemand;
            tbxSaveFolder.Text = Properties.Settings.Default.PathToSaveFolder;
            foreach (KeyValuePair<string, List<string>> entry in Configuration.GetAllFocus()) { cbxSelectFocus.Items.Add(entry); }
            cbxSelectFocus.DisplayMember = "Key";
        }

        #endregion

        #region List

        /// <summary>
        /// Function to load the strings of the newly selected parameter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParameterChanged(object sender, EventArgs e)
        {
            lbxStringsList.Items.Clear();
            switch (cbxSelectList.SelectedItem.ToString())
            {
                case AlertedProcesses:
                    foreach (string str in Configuration.GetAlertedProcesses()) { lbxStringsList.Items.Add(str); }
                    break;
                case AlertedUrls:
                    foreach (string str in Configuration.GetAlertedUrls()) { lbxStringsList.Items.Add(str); }
                    break;
                case IgnoredUrls:
                    foreach (string str in Configuration.GetIgnoredUrls()) { lbxStringsList.Items.Add(str); }
                    break;
                case IgnoredProcesses:
                    foreach (string str in Configuration.GetIgnoredProcesses()) { lbxStringsList.Items.Add(str); }
                    break;
                case AutorisedUrls:
                    foreach (string str in Configuration.GetAutorisedWebsite()) { lbxStringsList.Items.Add(str); }
                    break;
            }
        }

        /// <summary>
        /// Function that removes the selected string from the selected parameter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelectedString_Click(object sender, EventArgs e)
        {
            while (lbxStringsList.SelectedItems.Count > 0)
            { lbxStringsList.Items.Remove(lbxStringsList.SelectedItems[0]); }
            switch (cbxSelectList.SelectedItem.ToString())
            {
                case AlertedProcesses:
                    List<string> alertedProcesses = new();
                    foreach (string str in lbxStringsList.Items) { alertedProcesses.Add(str); }
                    Configuration.SetAlertedProcesses(alertedProcesses);
                    break;
                case AlertedUrls:
                    List<string> alertedUrls = new();
                    foreach (string str in lbxStringsList.Items) { alertedUrls.Add(str); }
                    Configuration.SetAlertedUrls(alertedUrls);
                    break;
                case IgnoredUrls:
                    List<string> ignoredUrls = new();
                    foreach (string str in lbxStringsList.Items) { ignoredUrls.Add(str); }
                    Configuration.SetIgnoredUrls(ignoredUrls);
                    break;
                case IgnoredProcesses:
                    List<string> ignoredProcesses = new();
                    foreach (string str in lbxStringsList.Items) { ignoredProcesses.Add(str); }
                    Configuration.SetIgnoredProcesses(ignoredProcesses);
                    break;
                case AutorisedUrls:
                    List<string> autorisedWebsite = new();
                    foreach (string str in lbxStringsList.Items) { autorisedWebsite.Add(str); }
                    Configuration.SetAutorisedWebsite(autorisedWebsite);
                    break;
            }

        }

        /// <summary>
        /// Function that add the string to the selected parameter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddString_Click(object sender, EventArgs e)
        {
            switch (cbxSelectList.SelectedItem.ToString())
            {
                case AlertedProcesses:
                    List<string> alertedProcesses = Configuration.GetAlertedProcesses();
                    alertedProcesses.Add(tbxAddStringList.Text);
                    Configuration.SetAlertedProcesses(alertedProcesses);
                    break;
                case AlertedUrls:
                    List<string> alertedUrls = Configuration.GetAlertedUrls();
                    alertedUrls.Add(tbxAddStringList.Text);
                    Configuration.SetAlertedUrls(alertedUrls);
                    break;
                case IgnoredUrls:
                    List<string> ignoredUrls = Configuration.GetIgnoredUrls();
                    ignoredUrls.Add(tbxAddStringList.Text);
                    Configuration.SetIgnoredUrls(ignoredUrls);
                    break;
                case IgnoredProcesses:
                    List<string> ignoredProcesses = Configuration.GetIgnoredProcesses();
                    ignoredProcesses.Add(tbxAddStringList.Text);
                    Configuration.SetIgnoredProcesses(ignoredProcesses);
                    break;
                case AutorisedUrls:
                    List<string> autorisedWebsite = Configuration.GetAutorisedWebsite();
                    autorisedWebsite.Add(tbxAddStringList.Text);
                    Configuration.SetAutorisedWebsite(autorisedWebsite);
                    break;
            }
            lbxStringsList.Items.Add(tbxAddStringList.Text);
            tbxAddStringList.Text = string.Empty;
        }

        #endregion

        #region Focus

        /// <summary>
        /// Function to add a new focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewFocus_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, List<string>> Focus = Configuration.GetAllFocus();
                Focus.Add("new focus", new());
                cbxSelectFocus.Items.Clear();
                foreach (KeyValuePair<string, List<string>> item in Focus) { cbxSelectFocus.Items.Add(item); }
                Configuration.SetAllFocus(Focus);
            }
            catch (Exception) {/* new focus is already created, the user needs to change it's name before creating another one.*/ }
        }

        /// <summary>
        /// Function to load the strings of the newly selected focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectedFocusChanged(object sender, EventArgs e)
        {
            Dictionary<string, List<string>> Focus = Configuration.GetAllFocus();
            string key = ((KeyValuePair<string, List<string>>)cbxSelectFocus.SelectedItem).Key;
            tbxFocusName.Text = key;
            lbxStringsFocus.Items.Clear();
            lbxStringsFocus.Items.AddRange(Focus[key].ToArray());
        }

        /// <summary>
        /// Fonction to handle the name change of a focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FocusNameChanged(object sender, EventArgs e)
        {
            Dictionary<string, List<string>> Focus = Configuration.GetAllFocus();
            string oldKey = ((KeyValuePair<string, List<string>>)cbxSelectFocus.SelectedItem).Key;

            Focus.Add(tbxFocusName.Text, Focus[oldKey]);
            Focus.Remove(oldKey);
            Configuration.SetAllFocus(Focus);
            cbxSelectFocus.Items.Clear();
            foreach (KeyValuePair<string, List<string>> entry in Configuration.GetAllFocus())
            {
                cbxSelectFocus.Items.Add(entry);
                if (entry.Key == tbxFocusName.Text) { cbxSelectFocus.SelectedItem = entry; }
            }
        }

        /// <summary>
        /// Function to add a string to the selected focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddStringToFocus(object sender, EventArgs e)
        {
            if (tbxAddStringFocus.Text == string.Empty) { return; }
            Dictionary<string, List<string>> Focus = Configuration.GetAllFocus();
            string key = ((KeyValuePair<string, List<string>>)cbxSelectFocus.SelectedItem).Key;
            Focus[key].Add(tbxAddStringFocus.Text);
            lbxStringsFocus.Items.Add(tbxAddStringFocus.Text);
            tbxAddStringFocus.Text = string.Empty;
            Configuration.SetAllFocus(Focus);
        }


        /// <summary>
        /// Function to remove all selected strings to the selected focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveStringToFocus(object sender, EventArgs e)
        {
            Dictionary<string, List<string>> Focus = Configuration.GetAllFocus();
            string key = ((KeyValuePair<string, List<string>>)cbxSelectFocus.SelectedItem).Key;
            while (lbxStringsFocus.SelectedItems.Count > 0)
            {
                Focus[key].Remove((string)lbxStringsFocus.SelectedItems[0]);
                lbxStringsFocus.Items.Remove(lbxStringsFocus.SelectedItems[0]);
            }
            Configuration.SetAllFocus(Focus);
        }

        #endregion

        #region Other Parameters

        /// <summary>
        /// Function to change the location of the save folder.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeSaveFolder_Click(object sender, EventArgs e)
        {
            DialogResult result = fbdSaveFolder.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbdSaveFolder.SelectedPath))
            {
                tbxSaveFolder.Text = fbdSaveFolder.SelectedPath;
                Properties.Settings.Default.PathToSaveFolder = fbdSaveFolder.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Function to update the time between asking update to the student.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTimeBetweenAsking(object sender, EventArgs e)
        {
            Properties.Settings.Default.TimeBetweenDemand = (int)nudTimeBetweenAsking.Value;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
