namespace ApplicationCliente
{
    partial class StudentApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudentApp));
            this.pbxScreeShot = new System.Windows.Forms.PictureBox();
            this.lbxConnexion = new System.Windows.Forms.ListBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnChangeIP = new System.Windows.Forms.Button();
            this.TrayIconStudent = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblMessage = new System.Windows.Forms.Label();
            this.lbxMessages = new System.Windows.Forms.ListBox();
            this.btnResetAllIP = new System.Windows.Forms.Button();
            this.btnWebView2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxScreeShot
            // 
            this.pbxScreeShot.Location = new System.Drawing.Point(16, 187);
            this.pbxScreeShot.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbxScreeShot.Name = "pbxScreeShot";
            this.pbxScreeShot.Size = new System.Drawing.Size(1580, 673);
            this.pbxScreeShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxScreeShot.TabIndex = 4;
            this.pbxScreeShot.TabStop = false;
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.ItemHeight = 16;
            this.lbxConnexion.Location = new System.Drawing.Point(16, 15);
            this.lbxConnexion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(1937, 164);
            this.lbxConnexion.TabIndex = 7;
            this.lbxConnexion.TabStop = false;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(1617, 194);
            this.btnHelp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(100, 28);
            this.btnHelp.TabIndex = 0;
            this.btnHelp.Text = "Aide";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.HelpReceive);
            // 
            // btnChangeIP
            // 
            this.btnChangeIP.Location = new System.Drawing.Point(1725, 194);
            this.btnChangeIP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnChangeIP.Name = "btnChangeIP";
            this.btnChangeIP.Size = new System.Drawing.Size(100, 28);
            this.btnChangeIP.TabIndex = 1;
            this.btnChangeIP.Text = "Changer l\'IP";
            this.btnChangeIP.UseVisualStyleBackColor = true;
            this.btnChangeIP.Click += new System.EventHandler(this.NewTeacherIP);
            // 
            // TrayIconStudent
            // 
            this.TrayIconStudent.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIconStudent.Icon")));
            this.TrayIconStudent.Text = "TrayIconStudent";
            this.TrayIconStudent.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconStudentClick);
            this.TrayIconStudent.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIconStudentClick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(1604, 277);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(64, 16);
            this.lblMessage.TabIndex = 3;
            this.lblMessage.Text = "Message";
            // 
            // lbxMessages
            // 
            this.lbxMessages.FormattingEnabled = true;
            this.lbxMessages.ItemHeight = 16;
            this.lbxMessages.Location = new System.Drawing.Point(1608, 297);
            this.lbxMessages.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lbxMessages.Name = "lbxMessages";
            this.lbxMessages.Size = new System.Drawing.Size(345, 324);
            this.lbxMessages.TabIndex = 4;
            this.lbxMessages.TabStop = false;
            // 
            // btnResetAllIP
            // 
            this.btnResetAllIP.Location = new System.Drawing.Point(1617, 230);
            this.btnResetAllIP.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnResetAllIP.Name = "btnResetAllIP";
            this.btnResetAllIP.Size = new System.Drawing.Size(208, 28);
            this.btnResetAllIP.TabIndex = 2;
            this.btnResetAllIP.Text = "Réinitialiser les IPs";
            this.btnResetAllIP.UseVisualStyleBackColor = true;
            // 
            // btnWebView2
            // 
            this.btnWebView2.Location = new System.Drawing.Point(1824, 629);
            this.btnWebView2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnWebView2.Name = "btnWebView2";
            this.btnWebView2.Size = new System.Drawing.Size(100, 28);
            this.btnWebView2.TabIndex = 8;
            this.btnWebView2.Text = "WebView2";
            this.btnWebView2.UseVisualStyleBackColor = true;
            this.btnWebView2.Click += new System.EventHandler(this.WebView2_Click);
            // 
            // StudentApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 875);
            this.Controls.Add(this.btnWebView2);
            this.Controls.Add(this.btnResetAllIP);
            this.Controls.Add(this.lbxMessages);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnChangeIP);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.pbxScreeShot);
            this.Controls.Add(this.lbxConnexion);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "StudentApp";
            this.Text = "Imepro";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosing);
            this.Resize += new System.EventHandler(this.StudentAppResized);
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pbxScreeShot;
        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnChangeIP;
        private System.Windows.Forms.NotifyIcon TrayIconStudent;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ListBox lbxMessages;
        private System.Windows.Forms.Button btnResetAllIP;
        private System.Windows.Forms.Button btnWebView2;
    }
}

