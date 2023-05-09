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
            this.button1 = new System.Windows.Forms.Button();
            this.TrayIconStudent = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblMessage = new System.Windows.Forms.Label();
            this.lbxMessages = new System.Windows.Forms.ListBox();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.lbxAutorisedWebSite = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxScreeShot
            // 
            this.pbxScreeShot.Location = new System.Drawing.Point(12, 152);
            this.pbxScreeShot.Name = "pbxScreeShot";
            this.pbxScreeShot.Size = new System.Drawing.Size(420, 306);
            this.pbxScreeShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
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
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.HelpReceive);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1294, 158);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Changer l\'IP";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.NewTeacherIP);
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
            this.lblMessage.Location = new System.Drawing.Point(1203, 208);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(50, 13);
            this.lblMessage.TabIndex = 8;
            this.lblMessage.Text = "Message";
            // 
            // lbxMessages
            // 
            this.lbxMessages.FormattingEnabled = true;
            this.lbxMessages.Location = new System.Drawing.Point(1206, 224);
            this.lbxMessages.Name = "lbxMessages";
            this.lbxMessages.Size = new System.Drawing.Size(260, 264);
            this.lbxMessages.TabIndex = 9;
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView.Location = new System.Drawing.Point(437, 152);
            this.webView.Name = "webView";
            this.webView.Size = new System.Drawing.Size(759, 547);
            this.webView.Source = new System.Uri("https://www.microsoft.com", System.UriKind.Absolute);
            this.webView.TabIndex = 10;
            this.webView.ZoomFactor = 1D;
            this.webView.NavigationStarting += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs>(this.ChangingUrl);
            this.webView.SourceChanged += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs>(this.webView_SourceChanged);
            // 
            // lbxAutorisedWebSite
            // 
            this.lbxAutorisedWebSite.FormattingEnabled = true;
            this.lbxAutorisedWebSite.Location = new System.Drawing.Point(12, 464);
            this.lbxAutorisedWebSite.Name = "lbxAutorisedWebSite";
            this.lbxAutorisedWebSite.Size = new System.Drawing.Size(419, 238);
            this.lbxAutorisedWebSite.TabIndex = 11;
            this.lbxAutorisedWebSite.SelectedIndexChanged += new System.EventHandler(this.ChangeWebSite);
            // 
            // StudentApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1474, 711);
            this.Controls.Add(this.lbxAutorisedWebSite);
            this.Controls.Add(this.webView);
            this.Controls.Add(this.lbxMessages);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.pbxScreeShot);
            this.Controls.Add(this.lbxConnexion);
            this.Name = "StudentApp";
            this.Text = "Student";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClosing);
            this.Resize += new System.EventHandler(this.StudentAppResized);
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pbxScreeShot;
        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NotifyIcon TrayIconStudent;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ListBox lbxMessages;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.ListBox lbxAutorisedWebSite;
    }
}

