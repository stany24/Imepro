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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeacherApp));
            this.lbxConnexion = new System.Windows.Forms.ListBox();
            this.lbxRequetes = new System.Windows.Forms.ListBox();
            this.pbxScreenShot = new System.Windows.Forms.PictureBox();
            this.btnShare = new System.Windows.Forms.Button();
            this.TrayIconTeacher = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblIP = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.TreeViewDetails = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TreeViewSelect = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(345, 655);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(200, 160);
            this.lbxConnexion.TabIndex = 4;
            // 
            // lbxRequetes
            // 
            this.lbxRequetes.FormattingEnabled = true;
            this.lbxRequetes.Location = new System.Drawing.Point(12, 655);
            this.lbxRequetes.Name = "lbxRequetes";
            this.lbxRequetes.Size = new System.Drawing.Size(311, 264);
            this.lbxRequetes.TabIndex = 5;
            // 
            // pbxScreenShot
            // 
            this.pbxScreenShot.Location = new System.Drawing.Point(564, 655);
            this.pbxScreenShot.Name = "pbxScreenShot";
            this.pbxScreenShot.Size = new System.Drawing.Size(200, 153);
            this.pbxScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxScreenShot.TabIndex = 7;
            this.pbxScreenShot.TabStop = false;
            // 
            // btnShare
            // 
            this.btnShare.Location = new System.Drawing.Point(12, 615);
            this.btnShare.Name = "btnShare";
            this.btnShare.Size = new System.Drawing.Size(94, 21);
            this.btnShare.TabIndex = 8;
            this.btnShare.Text = "Share Screen";
            this.btnShare.UseVisualStyleBackColor = true;
            this.btnShare.Click += new System.EventHandler(this.ShareScreen);
            // 
            // TrayIconTeacher
            // 
            this.TrayIconTeacher.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIconTeacher.Icon")));
            this.TrayIconTeacher.Text = "TrayIconTeacher";
            this.TrayIconTeacher.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconTeacherClick);
            this.TrayIconTeacher.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconTeacherClick);
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(212, 619);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 13);
            this.lblIP.TabIndex = 10;
            this.lblIP.Text = "IP:";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(112, 615);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(94, 21);
            this.btnDisconnect.TabIndex = 11;
            this.btnDisconnect.Text = "Disconect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.StopClient);
            // 
            // TreeViewDetails
            // 
            this.TreeViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewDetails.Location = new System.Drawing.Point(0, 0);
            this.TreeViewDetails.Name = "TreeViewDetails";
            this.TreeViewDetails.Size = new System.Drawing.Size(250, 588);
            this.TreeViewDetails.TabIndex = 12;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TreeViewDetails);
            this.splitContainer1.Panel1MinSize = 125;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.TreeViewSelect);
            this.splitContainer1.Panel2MinSize = 125;
            this.splitContainer1.Size = new System.Drawing.Size(752, 588);
            this.splitContainer1.SplitterDistance = 250;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitContainer1_SplitterMoved);
            // 
            // TreeViewSelect
            // 
            this.TreeViewSelect.CheckBoxes = true;
            this.TreeViewSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewSelect.Location = new System.Drawing.Point(0, 0);
            this.TreeViewSelect.Name = "TreeViewSelect";
            this.TreeViewSelect.Size = new System.Drawing.Size(498, 588);
            this.TreeViewSelect.TabIndex = 0;
            // 
            // TeacherApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 961);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.btnShare);
            this.Controls.Add(this.pbxScreenShot);
            this.Controls.Add(this.lbxRequetes);
            this.Controls.Add(this.lbxConnexion);
            this.Name = "TeacherApp";
            this.Text = "Teacher";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosing);
            this.SizeChanged += new System.EventHandler(this.TeacherAppResized);
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.ListBox lbxRequetes;
        private System.Windows.Forms.PictureBox pbxScreenShot;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.NotifyIcon TrayIconTeacher;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TreeView TreeViewDetails;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView TreeViewSelect;
    }
}

