using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class AskToChoseIp : Form
    {
        public IPAddress ChoosenIp;
        public AskToChoseIp(List<IPAddress> adresses)
        {
            InitializeComponent();
            foreach(IPAddress address in adresses){lbxAdresses.Items.Add(address);}
        }

        private void btnConfirmer_Click(object sender, EventArgs e)
        {
            ChoosenIp = lbxAdresses.SelectedItem as IPAddress;
        }

        private void lbxAdresses_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnConfirmer.Enabled = true;
        }

        private void lbxAdresses_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnConfirmer_Click(sender, e);
        }
    }
}
