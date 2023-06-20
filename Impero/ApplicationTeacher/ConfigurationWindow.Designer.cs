namespace ApplicationTeacher
{
    partial class ConfigurationWindow
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
            this.cbxLists = new System.Windows.Forms.ComboBox();
            this.lbxStrings = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddString = new System.Windows.Forms.Button();
            this.tbxAddString = new System.Windows.Forms.TextBox();
            this.lblList = new System.Windows.Forms.Label();
            this.lblAddedString = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbxLists
            // 
            this.cbxLists.FormattingEnabled = true;
            this.cbxLists.Location = new System.Drawing.Point(12, 33);
            this.cbxLists.Name = "cbxLists";
            this.cbxLists.Size = new System.Drawing.Size(156, 21);
            this.cbxLists.TabIndex = 0;
            this.cbxLists.SelectedIndexChanged += new System.EventHandler(this.cbxLists_SelectedIndexChanged);
            // 
            // lbxStrings
            // 
            this.lbxStrings.FormattingEnabled = true;
            this.lbxStrings.Location = new System.Drawing.Point(184, 12);
            this.lbxStrings.Name = "lbxStrings";
            this.lbxStrings.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxStrings.Size = new System.Drawing.Size(199, 368);
            this.lbxStrings.TabIndex = 1;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(93, 145);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Retirer";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAddString
            // 
            this.btnAddString.Location = new System.Drawing.Point(12, 145);
            this.btnAddString.Name = "btnAddString";
            this.btnAddString.Size = new System.Drawing.Size(75, 23);
            this.btnAddString.TabIndex = 3;
            this.btnAddString.Text = "Ajouter";
            this.btnAddString.UseVisualStyleBackColor = true;
            this.btnAddString.Click += new System.EventHandler(this.btnAddString_Click);
            // 
            // tbxAddString
            // 
            this.tbxAddString.Location = new System.Drawing.Point(12, 119);
            this.tbxAddString.Name = "tbxAddString";
            this.tbxAddString.Size = new System.Drawing.Size(156, 20);
            this.tbxAddString.TabIndex = 4;
            // 
            // lblList
            // 
            this.lblList.AutoSize = true;
            this.lblList.Location = new System.Drawing.Point(9, 9);
            this.lblList.Name = "lblList";
            this.lblList.Size = new System.Drawing.Size(77, 13);
            this.lblList.TabIndex = 5;
            this.lblList.Text = "Liste a modifier";
            // 
            // lblAddedString
            // 
            this.lblAddedString.AutoSize = true;
            this.lblAddedString.Location = new System.Drawing.Point(10, 103);
            this.lblAddedString.Name = "lblAddedString";
            this.lblAddedString.Size = new System.Drawing.Size(84, 13);
            this.lblAddedString.TabIndex = 6;
            this.lblAddedString.Text = "Chaine a ajouter";
            // 
            // ConfigurationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 391);
            this.Controls.Add(this.lblAddedString);
            this.Controls.Add(this.lblList);
            this.Controls.Add(this.tbxAddString);
            this.Controls.Add(this.btnAddString);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lbxStrings);
            this.Controls.Add(this.cbxLists);
            this.Name = "ConfigurationWindow";
            this.Text = "ConfigurationWindow";
            this.Load += new System.EventHandler(this.ConfigurationWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxLists;
        private System.Windows.Forms.ListBox lbxStrings;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddString;
        private System.Windows.Forms.TextBox tbxAddString;
        private System.Windows.Forms.Label lblList;
        private System.Windows.Forms.Label lblAddedString;
    }
}