namespace ApplicationTeacher
{
    partial class ChooseStudentToShareScreen
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
            this.lbxStudents = new System.Windows.Forms.ListBox();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnBeginSharing = new System.Windows.Forms.Button();
            this.lbxFocus = new System.Windows.Forms.ListBox();
            this.lbxPriorite = new System.Windows.Forms.ListBox();
            this.lblPriorite = new System.Windows.Forms.Label();
            this.lblEleves = new System.Windows.Forms.Label();
            this.lblFocus = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbxStudents
            // 
            this.lbxStudents.FormattingEnabled = true;
            this.lbxStudents.Location = new System.Drawing.Point(12, 25);
            this.lbxStudents.Name = "lbxStudents";
            this.lbxStudents.Size = new System.Drawing.Size(162, 212);
            this.lbxStudents.TabIndex = 0;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(12, 243);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(80, 23);
            this.btnSelectAll.TabIndex = 1;
            this.btnSelectAll.Text = "Tous";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.SelectAll);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Location = new System.Drawing.Point(98, 243);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(76, 23);
            this.btnSelectNone.TabIndex = 2;
            this.btnSelectNone.Text = "Aucun";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            this.btnSelectNone.Click += new System.EventHandler(this.SelectNone);
            // 
            // btnBeginSharing
            // 
            this.btnBeginSharing.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnBeginSharing.Location = new System.Drawing.Point(202, 243);
            this.btnBeginSharing.Name = "btnBeginSharing";
            this.btnBeginSharing.Size = new System.Drawing.Size(132, 23);
            this.btnBeginSharing.TabIndex = 3;
            this.btnBeginSharing.Text = "Commencer la diffusion";
            this.btnBeginSharing.UseVisualStyleBackColor = true;
            this.btnBeginSharing.Click += new System.EventHandler(this.btnBeginSharing_Click);
            // 
            // lbxFocus
            // 
            this.lbxFocus.FormattingEnabled = true;
            this.lbxFocus.Location = new System.Drawing.Point(180, 119);
            this.lbxFocus.Name = "lbxFocus";
            this.lbxFocus.Size = new System.Drawing.Size(179, 95);
            this.lbxFocus.TabIndex = 4;
            // 
            // lbxPriorite
            // 
            this.lbxPriorite.FormattingEnabled = true;
            this.lbxPriorite.Location = new System.Drawing.Point(180, 28);
            this.lbxPriorite.Name = "lbxPriorite";
            this.lbxPriorite.Size = new System.Drawing.Size(179, 69);
            this.lbxPriorite.TabIndex = 5;
            // 
            // lblPriorite
            // 
            this.lblPriorite.AutoSize = true;
            this.lblPriorite.Location = new System.Drawing.Point(181, 12);
            this.lblPriorite.Name = "lblPriorite";
            this.lblPriorite.Size = new System.Drawing.Size(39, 13);
            this.lblPriorite.TabIndex = 6;
            this.lblPriorite.Text = "Priorité";
            // 
            // lblEleves
            // 
            this.lblEleves.AutoSize = true;
            this.lblEleves.Location = new System.Drawing.Point(12, 12);
            this.lblEleves.Name = "lblEleves";
            this.lblEleves.Size = new System.Drawing.Size(39, 13);
            this.lblEleves.TabIndex = 7;
            this.lblEleves.Text = "Élèves";
            // 
            // lblFocus
            // 
            this.lblFocus.AutoSize = true;
            this.lblFocus.Location = new System.Drawing.Point(181, 103);
            this.lblFocus.Name = "lblFocus";
            this.lblFocus.Size = new System.Drawing.Size(36, 13);
            this.lblFocus.TabIndex = 8;
            this.lblFocus.Text = "Focus";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(181, 224);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 9;
            // 
            // ChooseStudentToShareScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 274);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.lblFocus);
            this.Controls.Add(this.lblEleves);
            this.Controls.Add(this.lblPriorite);
            this.Controls.Add(this.lbxPriorite);
            this.Controls.Add(this.lbxFocus);
            this.Controls.Add(this.btnBeginSharing);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.lbxStudents);
            this.Name = "ChooseStudentToShareScreen";
            this.Text = "ChooseStudentToShareScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxStudents;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnBeginSharing;
        private System.Windows.Forms.ListBox lbxFocus;
        private System.Windows.Forms.ListBox lbxPriorite;
        private System.Windows.Forms.Label lblPriorite;
        private System.Windows.Forms.Label lblEleves;
        private System.Windows.Forms.Label lblFocus;
        private System.Windows.Forms.Label lblError;
    }
}