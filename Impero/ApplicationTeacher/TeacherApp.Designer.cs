namespace ApplicationTeacher
{
    partial class TeacherApp
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbxConnexion = new System.Windows.Forms.ListBox();
            this.lbxRequetes = new System.Windows.Forms.ListBox();
            this.lbxClients = new System.Windows.Forms.ListBox();
            this.pbxScreenShot = new System.Windows.Forms.PictureBox();
            this.bthShare = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(12, 496);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(1072, 134);
            this.lbxConnexion.TabIndex = 4;
            // 
            // lbxRequetes
            // 
            this.lbxRequetes.FormattingEnabled = true;
            this.lbxRequetes.Location = new System.Drawing.Point(1090, 12);
            this.lbxRequetes.Name = "lbxRequetes";
            this.lbxRequetes.Size = new System.Drawing.Size(311, 615);
            this.lbxRequetes.TabIndex = 5;
            // 
            // lbxClients
            // 
            this.lbxClients.FormattingEnabled = true;
            this.lbxClients.Location = new System.Drawing.Point(12, 12);
            this.lbxClients.Name = "lbxClients";
            this.lbxClients.Size = new System.Drawing.Size(972, 134);
            this.lbxClients.TabIndex = 6;
            // 
            // pbxScreenShot
            // 
            this.pbxScreenShot.Location = new System.Drawing.Point(12, 152);
            this.pbxScreenShot.Name = "pbxScreenShot";
            this.pbxScreenShot.Size = new System.Drawing.Size(1072, 338);
            this.pbxScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxScreenShot.TabIndex = 7;
            this.pbxScreenShot.TabStop = false;
            // 
            // bthShare
            // 
            this.bthShare.Location = new System.Drawing.Point(990, 12);
            this.bthShare.Name = "bthShare";
            this.bthShare.Size = new System.Drawing.Size(94, 21);
            this.bthShare.TabIndex = 8;
            this.bthShare.Text = "Share Screen";
            this.bthShare.UseVisualStyleBackColor = true;
            this.bthShare.Click += new System.EventHandler(this.bthShare_Click);
            // 
            // TeacherApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 642);
            this.Controls.Add(this.bthShare);
            this.Controls.Add(this.pbxScreenShot);
            this.Controls.Add(this.lbxClients);
            this.Controls.Add(this.lbxRequetes);
            this.Controls.Add(this.lbxConnexion);
            this.Name = "TeacherApp";
            this.Text = "Teacher";
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.ListBox lbxRequetes;
        private System.Windows.Forms.ListBox lbxClients;
        private System.Windows.Forms.PictureBox pbxScreenShot;
        private System.Windows.Forms.Button bthShare;
    }
}

