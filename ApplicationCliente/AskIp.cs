using System;
using System.Net;
using System.Windows.Forms;

namespace ApplicationCliente
{
    public partial class AskIp : Form
    {
        private bool canClose = false;
        public AskIp()
        {
            InitializeComponent();
        }

        private void BtnConfirmer_Click(object sender, EventArgs e)
        {
            IpForTheWeek.SetIp(tbxIp.Text);
            canClose= true;
        }

        private void BtnHidden_Click(object sender, EventArgs e)
        {
            try {
                IPAddress.Parse(tbxIp.Text);
                btnConfirmer.Enabled= true;
                lblErreur.Text = "";
            }
            catch { lblErreur.Text = "Veuillez entrer un adresse correcte"; }
        }

        private void Ip_Changed(object sender, EventArgs e)
        {
            btnConfirmer.Enabled= false;
        }

        private void AskIp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!canClose) { e.Cancel = true; }
        }
    }
}
