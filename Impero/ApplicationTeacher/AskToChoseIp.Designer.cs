namespace ApplicationTeacher
{
    partial class AskToChoseIp
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
            this.lbxAdresses = new System.Windows.Forms.ListBox();
            this.btnConfirmer = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbxAdresses
            // 
            this.lbxAdresses.FormattingEnabled = true;
            this.lbxAdresses.Location = new System.Drawing.Point(12, 12);
            this.lbxAdresses.Name = "lbxAdresses";
            this.lbxAdresses.Size = new System.Drawing.Size(234, 186);
            this.lbxAdresses.TabIndex = 0;
            this.lbxAdresses.SelectedIndexChanged += new System.EventHandler(this.lbxAdresses_SelectedIndexChanged);
            this.lbxAdresses.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbxAdresses_MouseDoubleClick);
            // 
            // btnConfirmer
            // 
            this.btnConfirmer.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirmer.Enabled = false;
            this.btnConfirmer.Location = new System.Drawing.Point(94, 208);
            this.btnConfirmer.Name = "btnConfirmer";
            this.btnConfirmer.Size = new System.Drawing.Size(75, 23);
            this.btnConfirmer.TabIndex = 1;
            this.btnConfirmer.Text = "Confirmer";
            this.btnConfirmer.UseVisualStyleBackColor = true;
            this.btnConfirmer.Click += new System.EventHandler(this.btnConfirmer_Click);
            // 
            // AskToChoseIp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(258, 243);
            this.Controls.Add(this.btnConfirmer);
            this.Controls.Add(this.lbxAdresses);
            this.Name = "AskToChoseIp";
            this.Text = "Imepro IP";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbxAdresses;
        private System.Windows.Forms.Button btnConfirmer;
    }
}