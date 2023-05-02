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
            this.btnShare = new System.Windows.Forms.Button();
            this.TrayIconTeacher = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblIP = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.Slider = new System.Windows.Forms.TrackBar();
            this.TreeViewDetails = new System.Windows.Forms.TreeView();
            this.TreeViewSelect = new System.Windows.Forms.TreeView();
            this.panelMiniatures = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(268, 893);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(337, 56);
            this.lbxConnexion.TabIndex = 4;
            // 
            // lbxRequetes
            // 
            this.lbxRequetes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxRequetes.FormattingEnabled = true;
            this.lbxRequetes.Location = new System.Drawing.Point(611, 893);
            this.lbxRequetes.Name = "lbxRequetes";
            this.lbxRequetes.Size = new System.Drawing.Size(541, 56);
            this.lbxRequetes.TabIndex = 5;
            // 
            // btnShare
            // 
            this.btnShare.Location = new System.Drawing.Point(12, 567);
            this.btnShare.Name = "btnShare";
            this.btnShare.Size = new System.Drawing.Size(83, 21);
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
            this.lblIP.Location = new System.Drawing.Point(171, 571);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 13);
            this.lblIP.TabIndex = 10;
            this.lblIP.Text = "IP:";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(101, 567);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(64, 21);
            this.btnDisconnect.TabIndex = 11;
            this.btnDisconnect.Text = "Disconect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.StopClient);
            // 
            // Slider
            // 
            this.Slider.Location = new System.Drawing.Point(12, 594);
            this.Slider.Maximum = 100;
            this.Slider.Minimum = 5;
            this.Slider.Name = "Slider";
            this.Slider.Size = new System.Drawing.Size(250, 45);
            this.Slider.TabIndex = 12;
            this.Slider.Value = 10;
            this.Slider.Scroll += new System.EventHandler(this.Slider_Scroll);
            // 
            // TreeViewDetails
            // 
            this.TreeViewDetails.Location = new System.Drawing.Point(12, 12);
            this.TreeViewDetails.Name = "TreeViewDetails";
            this.TreeViewDetails.Size = new System.Drawing.Size(250, 549);
            this.TreeViewDetails.TabIndex = 12;
            // 
            // TreeViewSelect
            // 
            this.TreeViewSelect.CheckBoxes = true;
            this.TreeViewSelect.Location = new System.Drawing.Point(12, 642);
            this.TreeViewSelect.Name = "TreeViewSelect";
            this.TreeViewSelect.Size = new System.Drawing.Size(250, 307);
            this.TreeViewSelect.TabIndex = 0;
            this.TreeViewSelect.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeNodeChecked);
            // 
            // panelMiniatures
            // 
            this.panelMiniatures.AutoScroll = true;
            this.panelMiniatures.Location = new System.Drawing.Point(268, 12);
            this.panelMiniatures.Name = "panelMiniatures";
            this.panelMiniatures.Size = new System.Drawing.Size(1504, 875);
            this.panelMiniatures.TabIndex = 13;
            // 
            // TeacherApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 961);
            this.Controls.Add(this.panelMiniatures);
            this.Controls.Add(this.TreeViewDetails);
            this.Controls.Add(this.TreeViewSelect);
            this.Controls.Add(this.Slider);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.btnShare);
            this.Controls.Add(this.lbxRequetes);
            this.Controls.Add(this.lbxConnexion);
            this.Name = "TeacherApp";
            this.Text = "Teacher";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosing);
            this.SizeChanged += new System.EventHandler(this.TeacherAppResized);
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.ListBox lbxRequetes;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.NotifyIcon TrayIconTeacher;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.TrackBar Slider;
        private System.Windows.Forms.TreeView TreeViewDetails;
        private System.Windows.Forms.TreeView TreeViewSelect;
        private System.Windows.Forms.Panel panelMiniatures;
    }
}

