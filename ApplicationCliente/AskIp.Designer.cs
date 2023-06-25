namespace ApplicationCliente
{
    partial class AskIp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnConfirmer = new System.Windows.Forms.Button();
            this.tbxIp = new System.Windows.Forms.TextBox();
            this.lblErreur = new System.Windows.Forms.Label();
            this.bntVerifier = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(186, 13);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Entrez l\'adresse ip de votre professeur";
            // 
            // btnConfirmer
            // 
            this.btnConfirmer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirmer.Enabled = false;
            this.btnConfirmer.Location = new System.Drawing.Point(109, 51);
            this.btnConfirmer.Name = "btnConfirmer";
            this.btnConfirmer.Size = new System.Drawing.Size(87, 23);
            this.btnConfirmer.TabIndex = 1;
            this.btnConfirmer.Text = "Confirmer";
            this.btnConfirmer.UseVisualStyleBackColor = true;
            this.btnConfirmer.Click += new System.EventHandler(this.BtnConfirmer_Click);
            // 
            // tbxIp
            // 
            this.tbxIp.Location = new System.Drawing.Point(15, 25);
            this.tbxIp.Name = "tbxIp";
            this.tbxIp.Size = new System.Drawing.Size(181, 20);
            this.tbxIp.TabIndex = 2;
            this.tbxIp.TextChanged += new System.EventHandler(this.Ip_Changed);
            // 
            // lblErreur
            // 
            this.lblErreur.AutoSize = true;
            this.lblErreur.Location = new System.Drawing.Point(12, 90);
            this.lblErreur.Name = "lblErreur";
            this.lblErreur.Size = new System.Drawing.Size(0, 13);
            this.lblErreur.TabIndex = 3;
            // 
            // bntVerifier
            // 
            this.bntVerifier.Location = new System.Drawing.Point(15, 51);
            this.bntVerifier.Name = "bntVerifier";
            this.bntVerifier.Size = new System.Drawing.Size(88, 23);
            this.bntVerifier.TabIndex = 4;
            this.bntVerifier.Text = "Vérifier";
            this.bntVerifier.UseVisualStyleBackColor = true;
            this.bntVerifier.Click += new System.EventHandler(this.BtnHidden_Click);
            // 
            // AskIp
            // 
            this.AcceptButton = this.btnConfirmer;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 112);
            this.Controls.Add(this.bntVerifier);
            this.Controls.Add(this.lblErreur);
            this.Controls.Add(this.tbxIp);
            this.Controls.Add(this.btnConfirmer);
            this.Controls.Add(this.lblInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AskIp";
            this.Text = "Imepro IP";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AskIp_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnConfirmer;
        private System.Windows.Forms.TextBox tbxIp;
        private System.Windows.Forms.Label lblErreur;
        private System.Windows.Forms.Button bntVerifier;
    }
}