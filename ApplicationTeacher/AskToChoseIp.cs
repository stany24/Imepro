using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;

namespace ApplicationTeacher
{
    public partial class AskToChoseIp : Form
    {
        private IPAddress ChoosenIp;

        public IPAddress GetChoosenIp() { return ChoosenIp; }

        public AskToChoseIp(List<IPAddress> adresses)
        {
            InitializeComponent();
            foreach(IPAddress address in adresses){lbxAdresses.Items.Add(address);}
        }

        private void Confirmer_Click(object sender, EventArgs e)
        {
            ChoosenIp = lbxAdresses.SelectedItem as IPAddress;
        }

        private void SelectedIpAdressChanged(object sender, EventArgs e)
        {
            btnConfirmer.Enabled = true;
        }

        private void DoubleClickOnIpAdress(object sender, MouseEventArgs e)
        {
            Confirmer_Click(sender, e);
        }
    }
}
