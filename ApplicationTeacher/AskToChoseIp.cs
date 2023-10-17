using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class AskToChoseIp : Form
    {
        #region

        private IPAddress ChoosenIp;

        public IPAddress GetChoosenIp() { return ChoosenIp; }

        #endregion

        #region

        public AskToChoseIp(List<IPAddress> adresses)
        {
            InitializeComponent();
            foreach (IPAddress address in adresses) { lbxAdresses.Items.Add(address); }
        }

        #endregion

        #region Address

        /// <summary>
        /// Function that confirms the selected ip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirmer_Click(object sender, EventArgs e)
        {
            ChoosenIp = lbxAdresses.SelectedItem as IPAddress;
        }

        /// <summary>
        /// Function that enables the confirm button after selecting an ip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adresses_Changed(object sender, EventArgs e)
        {
            btnConfirmer.Enabled = true;
        }

        /// <summary>
        /// Function calling Confirmer_Click().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Adresses_DoubleClick(object sender, MouseEventArgs e)
        {
            Confirmer_Click(sender, e);
        }

        #endregion
    }
}
