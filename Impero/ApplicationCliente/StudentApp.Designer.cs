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
            this.btnFirefox = new System.Windows.Forms.Button();
            this.btnChrome = new System.Windows.Forms.Button();
            this.btnResetAllIP = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxScreeShot
            // 
            this.pbxScreeShot.Location = new System.Drawing.Point(12, 152);
            this.pbxScreeShot.Name = "pbxScreeShot";
            this.pbxScreeShot.Size = new System.Drawing.Size(1185, 547);
            this.pbxScreeShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxScreeShot.TabIndex = 4;
            this.pbxScreeShot.TabStop = false;
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(12, 12);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(1454, 134);
            this.lbxConnexion.TabIndex = 3;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(1213, 158);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 5;
            this.btnHelp.Text = "Aide";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.HelpReceive);
            // 
            // btnChangeIP
            // 
            this.btnChangeIP.Location = new System.Drawing.Point(1294, 158);
            this.btnChangeIP.Name = "btnChangeIP";
            this.btnChangeIP.Size = new System.Drawing.Size(75, 23);
            this.btnChangeIP.TabIndex = 6;
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
            this.lblMessage.Location = new System.Drawing.Point(1203, 225);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 8;
            this.lblMessage.Text = "Message";
            // 
            // lbxMessages
            // 
            this.lbxMessages.FormattingEnabled = true;
            this.lbxMessages.Location = new System.Drawing.Point(1206, 241);
            this.lbxMessages.Name = "lbxMessages";
            this.lbxMessages.Size = new System.Drawing.Size(260, 264);
            this.lbxMessages.TabIndex = 9;
            // 
            // btnFirefox
            // 
            this.btnFirefox.Location = new System.Drawing.Point(1206, 511);
            this.btnFirefox.Name = "btnFirefox";
            this.btnFirefox.Size = new System.Drawing.Size(75, 23);
            this.btnFirefox.TabIndex = 12;
            this.btnFirefox.Text = "Firefox";
            this.btnFirefox.UseVisualStyleBackColor = true;
            this.btnFirefox.Click += new System.EventHandler(this.NewFirefox);
            // 
            // btnChrome
            // 
            this.btnChrome.Location = new System.Drawing.Point(1287, 511);
            this.btnChrome.Name = "btnChrome";
            this.btnChrome.Size = new System.Drawing.Size(75, 23);
            this.btnChrome.TabIndex = 13;
            this.btnChrome.Text = "Chrome";
            this.btnChrome.UseVisualStyleBackColor = true;
            this.btnChrome.Click += new System.EventHandler(this.NewChrome);
            // 
            // btnResetAllIP
            // 
            this.btnResetAllIP.Location = new System.Drawing.Point(1213, 187);
            this.btnResetAllIP.Name = "btnResetAllIP";
            this.btnResetAllIP.Size = new System.Drawing.Size(156, 23);
            this.btnResetAllIP.TabIndex = 14;
            this.btnResetAllIP.Text = "Réinitialiser les IPs";
            this.btnResetAllIP.UseVisualStyleBackColor = true;
            this.btnResetAllIP.Click += new System.EventHandler(this.ResetAllIP_Click);
            // 
            // StudentApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1474, 711);
            this.Controls.Add(this.btnResetAllIP);
            this.Controls.Add(this.btnChrome);
            this.Controls.Add(this.btnFirefox);
            this.Controls.Add(this.lbxMessages);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnChangeIP);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.pbxScreeShot);
            this.Controls.Add(this.lbxConnexion);
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
        private System.Windows.Forms.Button btnFirefox;
        private System.Windows.Forms.Button btnChrome;
        private System.Windows.Forms.Button btnResetAllIP;
    }
}

