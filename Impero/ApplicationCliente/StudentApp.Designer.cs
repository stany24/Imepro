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
            this.pbxScreeShot = new System.Windows.Forms.PictureBox();
            this.lbxConnexion = new System.Windows.Forms.ListBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxScreeShot
            // 
            this.pbxScreeShot.Location = new System.Drawing.Point(12, 163);
            this.pbxScreeShot.Name = "pbxScreeShot";
            this.pbxScreeShot.Size = new System.Drawing.Size(1185, 536);
            this.pbxScreeShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxScreeShot.TabIndex = 4;
            this.pbxScreeShot.TabStop = false;
            // 
            // lbxConnexion
            // 
            this.lbxConnexion.FormattingEnabled = true;
            this.lbxConnexion.Location = new System.Drawing.Point(12, 12);
            this.lbxConnexion.Name = "lbxConnexion";
            this.lbxConnexion.Size = new System.Drawing.Size(1531, 134);
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
            // StudentApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1578, 711);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.pbxScreeShot);
            this.Controls.Add(this.lbxConnexion);
            this.Name = "StudentApp";
            this.Text = "Student";
            ((System.ComponentModel.ISupportInitialize)(this.pbxScreeShot)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pbxScreeShot;
        private System.Windows.Forms.ListBox lbxConnexion;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button button1;
    }
}

