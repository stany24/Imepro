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
            this.Slider = new System.Windows.Forms.TrackBar();
            this.TreeViewDetails = new System.Windows.Forms.TreeView();
            this.TreeViewSelect = new System.Windows.Forms.TreeView();
            this.panelMiniatures = new System.Windows.Forms.Panel();
            this.SplitterPrincipal = new System.Windows.Forms.SplitContainer();
            this.SplitterInfoTree = new System.Windows.Forms.SplitContainer();
            this.btnHideTreeView = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.SplitterTree = new System.Windows.Forms.SplitContainer();
            this.SplitterImageLog = new System.Windows.Forms.SplitContainer();
            this.SplitterConnexionRequetes = new System.Windows.Forms.SplitContainer();
            this.btnShowTreeView = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterPrincipal)).BeginInit();
            this.SplitterPrincipal.Panel1.SuspendLayout();
            this.SplitterPrincipal.Panel2.SuspendLayout();
            this.SplitterPrincipal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterInfoTree)).BeginInit();
            this.SplitterInfoTree.Panel1.SuspendLayout();
            this.SplitterInfoTree.Panel2.SuspendLayout();
            this.SplitterInfoTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterTree)).BeginInit();
            this.SplitterTree.Panel1.SuspendLayout();
            this.SplitterTree.Panel2.SuspendLayout();
            this.SplitterTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterImageLog)).BeginInit();
            this.SplitterImageLog.Panel1.SuspendLayout();
            this.SplitterImageLog.Panel2.SuspendLayout();
            this.SplitterImageLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterConnexionRequetes)).BeginInit();
            this.SplitterConnexionRequetes.Panel1.SuspendLayout();
            this.SplitterConnexionRequetes.Panel2.SuspendLayout();
            this.SplitterConnexionRequetes.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(0, 0);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(500, 57);
            this.lbxConnexion.TabIndex = 4;
            // 
            // lbxRequetes
            // 
            this.lbxRequetes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbxRequetes.FormattingEnabled = true;
            this.lbxRequetes.Location = new System.Drawing.Point(0, 0);
            this.lbxRequetes.Name = "lbxRequetes";
            this.lbxRequetes.Size = new System.Drawing.Size(996, 57);
            this.lbxRequetes.TabIndex = 5;
            // 
            // btnShare
            // 
            this.btnShare.Location = new System.Drawing.Point(3, 3);
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
            this.lblIP.Location = new System.Drawing.Point(12, 62);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 13);
            this.lblIP.TabIndex = 10;
            this.lblIP.Text = "IP:";
            // 
            // Slider
            // 
            this.Slider.Location = new System.Drawing.Point(3, 30);
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
            this.TreeViewDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewDetails.Location = new System.Drawing.Point(0, 0);
            this.TreeViewDetails.Name = "TreeViewDetails";
            this.TreeViewDetails.Size = new System.Drawing.Size(280, 417);
            this.TreeViewDetails.TabIndex = 12;
            // 
            // TreeViewSelect
            // 
            this.TreeViewSelect.CheckBoxes = true;
            this.TreeViewSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewSelect.Location = new System.Drawing.Point(0, 0);
            this.TreeViewSelect.Name = "TreeViewSelect";
            this.TreeViewSelect.Size = new System.Drawing.Size(280, 416);
            this.TreeViewSelect.TabIndex = 0;
            this.TreeViewSelect.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeNodeChecked);
            this.TreeViewSelect.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewDoubleClick);
            // 
            // panelMiniatures
            // 
            this.panelMiniatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMiniatures.Location = new System.Drawing.Point(0, 0);
            this.panelMiniatures.Name = "panelMiniatures";
            this.panelMiniatures.Size = new System.Drawing.Size(1500, 900);
            this.panelMiniatures.TabIndex = 13;
            this.panelMiniatures.Resize += new System.EventHandler(this.PanelMiniatures_Resize);
            // 
            // SplitterPrincipal
            // 
            this.SplitterPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterPrincipal.Location = new System.Drawing.Point(0, 0);
            this.SplitterPrincipal.Name = "SplitterPrincipal";
            // 
            // SplitterPrincipal.Panel1
            // 
            this.SplitterPrincipal.Panel1.Controls.Add(this.SplitterInfoTree);
            // 
            // SplitterPrincipal.Panel2
            // 
            this.SplitterPrincipal.Panel2.Controls.Add(this.SplitterImageLog);
            this.SplitterPrincipal.Size = new System.Drawing.Size(1784, 961);
            this.SplitterPrincipal.SplitterDistance = 280;
            this.SplitterPrincipal.TabIndex = 14;
            // 
            // SplitterInfoTree
            // 
            this.SplitterInfoTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterInfoTree.Location = new System.Drawing.Point(0, 0);
            this.SplitterInfoTree.Name = "SplitterInfoTree";
            this.SplitterInfoTree.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitterInfoTree.Panel1
            // 
            this.SplitterInfoTree.Panel1.Controls.Add(this.btnShowTreeView);
            this.SplitterInfoTree.Panel1.Controls.Add(this.btnHideTreeView);
            this.SplitterInfoTree.Panel1.Controls.Add(this.btnFilter);
            this.SplitterInfoTree.Panel1.Controls.Add(this.btnShare);
            this.SplitterInfoTree.Panel1.Controls.Add(this.lblIP);
            this.SplitterInfoTree.Panel1.Controls.Add(this.Slider);
            // 
            // SplitterInfoTree.Panel2
            // 
            this.SplitterInfoTree.Panel2.Controls.Add(this.SplitterTree);
            this.SplitterInfoTree.Size = new System.Drawing.Size(280, 961);
            this.SplitterInfoTree.SplitterDistance = 120;
            this.SplitterInfoTree.TabIndex = 14;
            // 
            // btnHideTreeView
            // 
            this.btnHideTreeView.Location = new System.Drawing.Point(12, 81);
            this.btnHideTreeView.Name = "btnHideTreeView";
            this.btnHideTreeView.Size = new System.Drawing.Size(85, 21);
            this.btnHideTreeView.TabIndex = 14;
            this.btnHideTreeView.Text = "Masquer tout";
            this.btnHideTreeView.UseVisualStyleBackColor = true;
            this.btnHideTreeView.Click += new System.EventHandler(this.HideTreeView_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(92, 3);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(120, 21);
            this.btnFilter.TabIndex = 13;
            this.btnFilter.Text = "Désactiver Les filtres";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.ButtonFilter_Click);
            // 
            // SplitterTree
            // 
            this.SplitterTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterTree.Location = new System.Drawing.Point(0, 0);
            this.SplitterTree.Name = "SplitterTree";
            this.SplitterTree.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitterTree.Panel1
            // 
            this.SplitterTree.Panel1.Controls.Add(this.TreeViewDetails);
            // 
            // SplitterTree.Panel2
            // 
            this.SplitterTree.Panel2.Controls.Add(this.TreeViewSelect);
            this.SplitterTree.Size = new System.Drawing.Size(280, 837);
            this.SplitterTree.SplitterDistance = 417;
            this.SplitterTree.TabIndex = 13;
            // 
            // SplitterImageLog
            // 
            this.SplitterImageLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterImageLog.Location = new System.Drawing.Point(0, 0);
            this.SplitterImageLog.Name = "SplitterImageLog";
            this.SplitterImageLog.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitterImageLog.Panel1
            // 
            this.SplitterImageLog.Panel1.Controls.Add(this.panelMiniatures);
            // 
            // SplitterImageLog.Panel2
            // 
            this.SplitterImageLog.Panel2.Controls.Add(this.SplitterConnexionRequetes);
            this.SplitterImageLog.Size = new System.Drawing.Size(1500, 961);
            this.SplitterImageLog.SplitterDistance = 900;
            this.SplitterImageLog.TabIndex = 14;
            // 
            // SplitterConnexionRequetes
            // 
            this.SplitterConnexionRequetes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterConnexionRequetes.Location = new System.Drawing.Point(0, 0);
            this.SplitterConnexionRequetes.Name = "SplitterConnexionRequetes";
            // 
            // SplitterConnexionRequetes.Panel1
            // 
            this.SplitterConnexionRequetes.Panel1.Controls.Add(this.lbxConnexion);
            // 
            // SplitterConnexionRequetes.Panel2
            // 
            this.SplitterConnexionRequetes.Panel2.Controls.Add(this.lbxRequetes);
            this.SplitterConnexionRequetes.Size = new System.Drawing.Size(1500, 57);
            this.SplitterConnexionRequetes.SplitterDistance = 500;
            this.SplitterConnexionRequetes.TabIndex = 6;
            // 
            // btnShowTreeView
            // 
            this.btnShowTreeView.Location = new System.Drawing.Point(103, 81);
            this.btnShowTreeView.Name = "btnShowTreeView";
            this.btnShowTreeView.Size = new System.Drawing.Size(85, 21);
            this.btnShowTreeView.TabIndex = 15;
            this.btnShowTreeView.Text = "Afficher Tout";
            this.btnShowTreeView.UseVisualStyleBackColor = true;
            this.btnShowTreeView.Click += new System.EventHandler(this.ShowTreeView_Click);
            // 
            // TeacherApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1784, 961);
            this.Controls.Add(this.SplitterPrincipal);
            this.Name = "TeacherApp";
            this.Text = "Teacher";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosing);
            this.SizeChanged += new System.EventHandler(this.TeacherAppResized);
            ((System.ComponentModel.ISupportInitialize)(this.Slider)).EndInit();
            this.SplitterPrincipal.Panel1.ResumeLayout(false);
            this.SplitterPrincipal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterPrincipal)).EndInit();
            this.SplitterPrincipal.ResumeLayout(false);
            this.SplitterInfoTree.Panel1.ResumeLayout(false);
            this.SplitterInfoTree.Panel1.PerformLayout();
            this.SplitterInfoTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterInfoTree)).EndInit();
            this.SplitterInfoTree.ResumeLayout(false);
            this.SplitterTree.Panel1.ResumeLayout(false);
            this.SplitterTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterTree)).EndInit();
            this.SplitterTree.ResumeLayout(false);
            this.SplitterImageLog.Panel1.ResumeLayout(false);
            this.SplitterImageLog.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterImageLog)).EndInit();
            this.SplitterImageLog.ResumeLayout(false);
            this.SplitterConnexionRequetes.Panel1.ResumeLayout(false);
            this.SplitterConnexionRequetes.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterConnexionRequetes)).EndInit();
            this.SplitterConnexionRequetes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.ListBox lbxRequetes;
        private System.Windows.Forms.Button btnShare;
        private System.Windows.Forms.NotifyIcon TrayIconTeacher;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TrackBar Slider;
        private System.Windows.Forms.TreeView TreeViewDetails;
        private System.Windows.Forms.TreeView TreeViewSelect;
        private System.Windows.Forms.Panel panelMiniatures;
        private System.Windows.Forms.SplitContainer SplitterPrincipal;
        private System.Windows.Forms.SplitContainer SplitterImageLog;
        private System.Windows.Forms.SplitContainer SplitterConnexionRequetes;
        private System.Windows.Forms.SplitContainer SplitterTree;
        private System.Windows.Forms.SplitContainer SplitterInfoTree;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnHideTreeView;
        private System.Windows.Forms.Button btnShowTreeView;
    }
}

