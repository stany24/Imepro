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
            this.lbxUrls = new System.Windows.Forms.ListBox();
            this.lbxProcesses = new System.Windows.Forms.ListBox();
            this.pbxScreenShot = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).BeginInit();
            this.SuspendLayout();
            // 
            // btnKillProcess
            // 
            this.btnKillProcess.Location = new System.Drawing.Point(17, 128);
            this.btnKillProcess.Name = "btnKillProcess";
            this.btnKillProcess.Size = new System.Drawing.Size(107, 23);
            this.btnKillProcess.TabIndex = 29;
            this.btnKillProcess.Text = "Tuer le processus";
            this.btnKillProcess.UseVisualStyleBackColor = true;
            // 
            // lblPoste
            // 
            this.lblPoste.AutoSize = true;
            this.lblPoste.Location = new System.Drawing.Point(14, 65);
            this.lblPoste.Name = "lblPoste";
            this.lblPoste.Size = new System.Drawing.Size(34, 13);
            this.lblPoste.TabIndex = 28;
            this.lblPoste.Text = "Poste";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(14, 16);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(29, 13);
            this.lblUserName.TabIndex = 27;
            this.lblUserName.Text = "Nom";
            // 
            // lbxUrls
            // 
            this.lbxUrls.FormattingEnabled = true;
            this.lbxUrls.Location = new System.Drawing.Point(184, 12);
            this.lbxUrls.Name = "lbxUrls";
            this.lbxUrls.Size = new System.Drawing.Size(419, 82);
            this.lbxUrls.TabIndex = 26;
            // 
            // lbxProcesses
            // 
            this.lbxProcesses.FormattingEnabled = true;
            this.lbxProcesses.Location = new System.Drawing.Point(181, 100);
            this.lbxProcesses.Name = "lbxProcesses";
            this.lbxProcesses.Size = new System.Drawing.Size(419, 82);
            this.lbxProcesses.TabIndex = 25;
            // 
            // pbxScreenShot
            // 
            this.pbxScreenShot.Image = ((System.Drawing.Image)(resources.GetObject("pbxScreenShot.Image")));
            this.pbxScreenShot.Location = new System.Drawing.Point(17, 188);
            this.pbxScreenShot.Name = "pbxScreenShot";
            this.pbxScreenShot.Size = new System.Drawing.Size(586, 276);
            this.pbxScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxScreenShot.TabIndex = 24;
            this.pbxScreenShot.TabStop = false;
            // 
            // DisplayStudent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 473);
            this.Controls.Add(this.btnKillProcess);
            this.Controls.Add(this.lblPoste);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lbxUrls);
            this.Controls.Add(this.lbxProcesses);
            this.Controls.Add(this.pbxScreenShot);
            this.Name = "DisplayStudent";
            this.Text = "DisplayStudent";
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreenShot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnKillProcess;
        private System.Windows.Forms.Label lblPoste;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.ListBox lbxUrls;
        private System.Windows.Forms.ListBox lbxProcesses;
        private System.Windows.Forms.PictureBox pbxScreenShot;
    }
}