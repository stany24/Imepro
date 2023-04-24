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
            this.components = new System.ComponentModel.Container();
            this.lbxConnexion = new System.Windows.Forms.ListBox();
            this.lbxRequetes = new System.Windows.Forms.ListBox();
            this.lbxClients = new System.Windows.Forms.ListBox();
            this.pbxScreenShot = new System.Windows.Forms.PictureBox();
            this.btnShare = new System.Windows.Forms.Button();
            this.TrayIconTeacher = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblStudents = new System.Windows.Forms.ListBox();
            this.lblIP = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(7, 456);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(1077, 160);
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
            this.lbxClients.Location = new System.Drawing.Point(176, 12);
            this.lbxClients.Name = "lbxClients";
            this.lbxClients.Size = new System.Drawing.Size(808, 134);
            this.lbxClients.TabIndex = 6;
            // 
            // pbxScreenShot
            // 
            this.pbxScreenShot.Location = new System.Drawing.Point(7, 152);
            this.pbxScreenShot.Name = "pbxScreenShot";
            this.pbxScreenShot.Size = new System.Drawing.Size(1077, 298);
            this.pbxScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxScreenShot.TabIndex = 7;
            this.pbxScreenShot.TabStop = false;
            // 
            // btnShare
            // 
            this.btnShare.Location = new System.Drawing.Point(990, 12);
            this.btnShare.Name = "btnShare";
            this.btnShare.Size = new System.Drawing.Size(94, 21);
            this.btnShare.TabIndex = 8;
            this.btnShare.Text = "Share Screen";
            this.btnShare.UseVisualStyleBackColor = true;
            this.btnShare.Click += new System.EventHandler(this.ShareScreen);
            // 
            // TrayIconTeacher
            // 
            this.TrayIconTeacher.Text = "TrayIconTeacher";
            this.TrayIconTeacher.Visible = true;
            this.TrayIconTeacher.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconTeacherClick);
            this.TrayIconTeacher.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconTeacherClick);
            // 
            // lblStudents
            // 
            this.lblStudents.FormattingEnabled = true;
            this.lblStudents.Location = new System.Drawing.Point(7, 12);
            this.lblStudents.Name = "lblStudents";
            this.lblStudents.Size = new System.Drawing.Size(163, 134);
            this.lblStudents.TabIndex = 9;
            this.lblStudents.SelectedIndexChanged += new System.EventHandler(this.SelectedStudentChanged);
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(4, 620);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 13);
            this.lblIP.TabIndex = 10;
            this.lblIP.Text = "IP:";
            // 
            // TeacherApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 642);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.lblStudents);
            this.Controls.Add(this.btnShare);
            this.Controls.Add(this.pbxScreenShot);
            this.Controls.Add(this.lbxClients);
            this.Controls.Add(this.lbxRequetes);
            this.Controls.Add(this.lbxConnexion);
            this.Name = "TeacherApp";
            this.Text = "Teacher";
            this.SizeChanged += new System.EventHandler(this.TeacherAppResized);
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.ListBox lbxRequetes;
        private System.Windows.Forms.ListBox lbxClients;
        private System.Windows.Forms.PictureBox pbxScreenShot;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.NotifyIcon TrayIconTeacher;
        private System.Windows.Forms.ListBox lblStudents;
        private System.Windows.Forms.Label lblIP;
    }
}

