namespace ApplicationTeacher
{
    partial class DisplayStudent
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayStudent));
            this.btnKillProcess = new System.Windows.Forms.Button();
            this.lblPoste = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.pbxScreenShot = new System.Windows.Forms.PictureBox();
            this.TreeViewUrls = new System.Windows.Forms.TreeView();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.tbxMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.TreeViewProcesses = new System.Windows.Forms.TreeView();
            this.SplitterImage = new System.Windows.Forms.SplitContainer();
            this.SplitterTreeInfo = new System.Windows.Forms.SplitContainer();
            this.SplitterInfoMessage = new System.Windows.Forms.SplitContainer();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnTakeControl = new System.Windows.Forms.Button();
            this.SplitterMessageButton = new System.Windows.Forms.SplitContainer();
            this.SplitterTreeTree = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterImage)).BeginInit();
            this.SplitterImage.Panel1.SuspendLayout();
            this.SplitterImage.Panel2.SuspendLayout();
            this.SplitterImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterTreeInfo)).BeginInit();
            this.SplitterTreeInfo.Panel1.SuspendLayout();
            this.SplitterTreeInfo.Panel2.SuspendLayout();
            this.SplitterTreeInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterInfoMessage)).BeginInit();
            this.SplitterInfoMessage.Panel1.SuspendLayout();
            this.SplitterInfoMessage.Panel2.SuspendLayout();
            this.SplitterInfoMessage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterMessageButton)).BeginInit();
            this.SplitterMessageButton.Panel1.SuspendLayout();
            this.SplitterMessageButton.Panel2.SuspendLayout();
            this.SplitterMessageButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitterTreeTree)).BeginInit();
            this.SplitterTreeTree.Panel1.SuspendLayout();
            this.SplitterTreeTree.Panel2.SuspendLayout();
            this.SplitterTreeTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnKillProcess
            // 
            this.btnKillProcess.Location = new System.Drawing.Point(168, 9);
            this.btnKillProcess.Name = "btnKillProcess";
            this.btnKillProcess.Size = new System.Drawing.Size(119, 23);
            this.btnKillProcess.TabIndex = 29;
            this.btnKillProcess.Text = "Tuer le processus";
            this.btnKillProcess.UseVisualStyleBackColor = true;
            this.btnKillProcess.Click += new System.EventHandler(this.KillProcess_Click);
            // 
            // lblPoste
            // 
            this.lblPoste.AutoSize = true;
            this.lblPoste.Location = new System.Drawing.Point(12, 36);
            this.lblPoste.Name = "lblPoste";
            this.lblPoste.Size = new System.Drawing.Size(34, 13);
            this.lblPoste.TabIndex = 28;
            this.lblPoste.Text = "Poste";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(12, 9);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(29, 13);
            this.lblUserName.TabIndex = 27;
            this.lblUserName.Text = "Nom";
            // 
            // pbxScreenShot
            // 
            this.pbxScreenShot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxScreenShot.Image = ((System.Drawing.Image)(resources.GetObject("pbxScreenShot.Image")));
            this.pbxScreenShot.Location = new System.Drawing.Point(0, 0);
            this.pbxScreenShot.Name = "pbxScreenShot";
            this.pbxScreenShot.Size = new System.Drawing.Size(1476, 523);
            this.pbxScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxScreenShot.TabIndex = 24;
            this.pbxScreenShot.TabStop = false;
            // 
            // TreeViewUrls
            // 
            this.TreeViewUrls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewUrls.Location = new System.Drawing.Point(0, 0);
            this.TreeViewUrls.Name = "TreeViewUrls";
            this.TreeViewUrls.Size = new System.Drawing.Size(980, 126);
            this.TreeViewUrls.TabIndex = 30;
            // 
            // btnSaveImage
            // 
            this.btnSaveImage.Location = new System.Drawing.Point(168, 36);
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.Size = new System.Drawing.Size(119, 23);
            this.btnSaveImage.TabIndex = 31;
            this.btnSaveImage.Text = "Sauvegarder l\'image";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.SaveScreenShot);
            // 
            // tbxMessage
            // 
            this.tbxMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxMessage.Location = new System.Drawing.Point(0, 0);
            this.tbxMessage.Multiline = true;
            this.tbxMessage.Name = "tbxMessage";
            this.tbxMessage.Size = new System.Drawing.Size(1005, 70);
            this.tbxMessage.TabIndex = 32;
            // 
            // btnSend
            // 
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSend.Location = new System.Drawing.Point(0, 0);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(63, 70);
            this.btnSend.TabIndex = 33;
            this.btnSend.Text = "Envoyer";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.SendMessage);
            // 
            // TreeViewProcesses
            // 
            this.TreeViewProcesses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeViewProcesses.Location = new System.Drawing.Point(0, 0);
            this.TreeViewProcesses.Name = "TreeViewProcesses";
            this.TreeViewProcesses.Size = new System.Drawing.Size(492, 126);
            this.TreeViewProcesses.TabIndex = 34;
            // 
            // SplitterImage
            // 
            this.SplitterImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterImage.Location = new System.Drawing.Point(0, 0);
            this.SplitterImage.Name = "SplitterImage";
            this.SplitterImage.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitterImage.Panel1
            // 
            this.SplitterImage.Panel1.Controls.Add(this.SplitterTreeInfo);
            // 
            // SplitterImage.Panel2
            // 
            this.SplitterImage.Panel2.Controls.Add(this.pbxScreenShot);
            this.SplitterImage.Size = new System.Drawing.Size(1476, 727);
            this.SplitterImage.SplitterDistance = 200;
            this.SplitterImage.TabIndex = 35;
            // 
            // SplitterTreeInfo
            // 
            this.SplitterTreeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterTreeInfo.Location = new System.Drawing.Point(0, 0);
            this.SplitterTreeInfo.Name = "SplitterTreeInfo";
            this.SplitterTreeInfo.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitterTreeInfo.Panel1
            // 
            this.SplitterTreeInfo.Panel1.Controls.Add(this.SplitterInfoMessage);
            // 
            // SplitterTreeInfo.Panel2
            // 
            this.SplitterTreeInfo.Panel2.Controls.Add(this.SplitterTreeTree);
            this.SplitterTreeInfo.Size = new System.Drawing.Size(1476, 200);
            this.SplitterTreeInfo.SplitterDistance = 70;
            this.SplitterTreeInfo.TabIndex = 0;
            // 
            // SplitterInfoMessage
            // 
            this.SplitterInfoMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterInfoMessage.Location = new System.Drawing.Point(0, 0);
            this.SplitterInfoMessage.Name = "SplitterInfoMessage";
            // 
            // SplitterInfoMessage.Panel1
            // 
            this.SplitterInfoMessage.Panel1.Controls.Add(this.btnStop);
            this.SplitterInfoMessage.Panel1.Controls.Add(this.btnTakeControl);
            this.SplitterInfoMessage.Panel1.Controls.Add(this.lblUserName);
            this.SplitterInfoMessage.Panel1.Controls.Add(this.lblPoste);
            this.SplitterInfoMessage.Panel1.Controls.Add(this.btnKillProcess);
            this.SplitterInfoMessage.Panel1.Controls.Add(this.btnSaveImage);
            // 
            // SplitterInfoMessage.Panel2
            // 
            this.SplitterInfoMessage.Panel2.Controls.Add(this.SplitterMessageButton);
            this.SplitterInfoMessage.Size = new System.Drawing.Size(1476, 70);
            this.SplitterInfoMessage.SplitterDistance = 400;
            this.SplitterInfoMessage.TabIndex = 0;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(294, 36);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(104, 23);
            this.btnStop.TabIndex = 33;
            this.btnStop.Text = "Arrêter";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.ShutDown_Click);
            // 
            // btnTakeControl
            // 
            this.btnTakeControl.Location = new System.Drawing.Point(293, 9);
            this.btnTakeControl.Name = "btnTakeControl";
            this.btnTakeControl.Size = new System.Drawing.Size(104, 23);
            this.btnTakeControl.TabIndex = 32;
            this.btnTakeControl.Text = "Prendre le control";
            this.btnTakeControl.UseVisualStyleBackColor = true;
            this.btnTakeControl.Click += new System.EventHandler(this.TakeControl_Click);
            // 
            // SplitterMessageButton
            // 
            this.SplitterMessageButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterMessageButton.Location = new System.Drawing.Point(0, 0);
            this.SplitterMessageButton.Name = "SplitterMessageButton";
            // 
            // SplitterMessageButton.Panel1
            // 
            this.SplitterMessageButton.Panel1.Controls.Add(this.tbxMessage);
            // 
            // SplitterMessageButton.Panel2
            // 
            this.SplitterMessageButton.Panel2.Controls.Add(this.btnSend);
            this.SplitterMessageButton.Size = new System.Drawing.Size(1072, 70);
            this.SplitterMessageButton.SplitterDistance = 1005;
            this.SplitterMessageButton.TabIndex = 36;
            // 
            // SplitterTreeTree
            // 
            this.SplitterTreeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitterTreeTree.Location = new System.Drawing.Point(0, 0);
            this.SplitterTreeTree.Name = "SplitterTreeTree";
            // 
            // SplitterTreeTree.Panel1
            // 
            this.SplitterTreeTree.Panel1.Controls.Add(this.TreeViewProcesses);
            // 
            // SplitterTreeTree.Panel2
            // 
            this.SplitterTreeTree.Panel2.Controls.Add(this.TreeViewUrls);
            this.SplitterTreeTree.Size = new System.Drawing.Size(1476, 126);
            this.SplitterTreeTree.SplitterDistance = 492;
            this.SplitterTreeTree.TabIndex = 0;
            // 
            // DisplayStudent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1476, 727);
            this.Controls.Add(this.SplitterImage);
            this.Name = "DisplayStudent";
            this.Text = "Imepro Individuel";
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).EndInit();
            this.SplitterImage.Panel1.ResumeLayout(false);
            this.SplitterImage.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterImage)).EndInit();
            this.SplitterImage.ResumeLayout(false);
            this.SplitterTreeInfo.Panel1.ResumeLayout(false);
            this.SplitterTreeInfo.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterTreeInfo)).EndInit();
            this.SplitterTreeInfo.ResumeLayout(false);
            this.SplitterInfoMessage.Panel1.ResumeLayout(false);
            this.SplitterInfoMessage.Panel1.PerformLayout();
            this.SplitterInfoMessage.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterInfoMessage)).EndInit();
            this.SplitterInfoMessage.ResumeLayout(false);
            this.SplitterMessageButton.Panel1.ResumeLayout(false);
            this.SplitterMessageButton.Panel1.PerformLayout();
            this.SplitterMessageButton.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterMessageButton)).EndInit();
            this.SplitterMessageButton.ResumeLayout(false);
            this.SplitterTreeTree.Panel1.ResumeLayout(false);
            this.SplitterTreeTree.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitterTreeTree)).EndInit();
            this.SplitterTreeTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnKillProcess;
        private System.Windows.Forms.Label lblPoste;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.PictureBox pbxScreenShot;
        private System.Windows.Forms.TreeView TreeViewUrls;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.TextBox tbxMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TreeView TreeViewProcesses;
        private System.Windows.Forms.SplitContainer SplitterImage;
        private System.Windows.Forms.SplitContainer SplitterTreeInfo;
        private System.Windows.Forms.SplitContainer SplitterInfoMessage;
        private System.Windows.Forms.SplitContainer SplitterMessageButton;
        private System.Windows.Forms.SplitContainer SplitterTreeTree;
        private System.Windows.Forms.Button btnTakeControl;
        private System.Windows.Forms.Button btnStop;
    }
}