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
            this.cbxCategory = new System.Windows.Forms.ComboBox();
            this.lbxStrings = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAddString = new System.Windows.Forms.Button();
            this.tbxAddString = new System.Windows.Forms.TextBox();
            this.lblChooseCategory = new System.Windows.Forms.Label();
            this.lblAddedString = new System.Windows.Forms.Label();
            this.lblTimeBetweenAsking = new System.Windows.Forms.Label();
            this.nudTimeBetweenAsking = new System.Windows.Forms.NumericUpDown();
            this.lblParameter = new System.Windows.Forms.Label();
            this.cbxParameter = new System.Windows.Forms.ComboBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblSaveFolder = new System.Windows.Forms.Label();
            this.fbdSaveFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnChangeSaveFolder = new System.Windows.Forms.Button();
            this.tbxSaveFolder = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeBetweenAsking)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxCategory
            // 
            this.cbxCategory.FormattingEnabled = true;
            this.cbxCategory.Location = new System.Drawing.Point(12, 33);
            this.cbxCategory.Name = "cbxCategory";
            this.cbxCategory.Size = new System.Drawing.Size(156, 21);
            this.cbxCategory.TabIndex = 0;
            this.cbxCategory.SelectedIndexChanged += new System.EventHandler(this.CategoryChanged);
            // 
            // lbxStrings
            // 
            this.lbxStrings.FormattingEnabled = true;
            this.lbxStrings.Location = new System.Drawing.Point(184, 12);
            this.lbxStrings.Name = "lbxStrings";
            this.lbxStrings.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxStrings.Size = new System.Drawing.Size(199, 368);
            this.lbxStrings.TabIndex = 12;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(95, 212);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Retirer";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.RemoveSelectedString_Click);
            // 
            // btnAddString
            // 
            this.btnAddString.Location = new System.Drawing.Point(14, 212);
            this.btnAddString.Name = "btnAddString";
            this.btnAddString.Size = new System.Drawing.Size(75, 23);
            this.btnAddString.TabIndex = 3;
            this.btnAddString.Text = "Ajouter";
            this.btnAddString.UseVisualStyleBackColor = true;
            this.btnAddString.Click += new System.EventHandler(this.AddString_Click);
            // 
            // tbxAddString
            // 
            this.tbxAddString.Location = new System.Drawing.Point(14, 186);
            this.tbxAddString.Name = "tbxAddString";
            this.tbxAddString.Size = new System.Drawing.Size(156, 20);
            this.tbxAddString.TabIndex = 2;
            // 
            // lblChooseCategory
            // 
            this.lblChooseCategory.AutoSize = true;
            this.lblChooseCategory.Location = new System.Drawing.Point(9, 9);
            this.lblChooseCategory.Name = "lblChooseCategory";
            this.lblChooseCategory.Size = new System.Drawing.Size(96, 13);
            this.lblChooseCategory.TabIndex = 9;
            this.lblChooseCategory.Text = "Choisir la categorie";
            // 
            // lblAddedString
            // 
            this.lblAddedString.AutoSize = true;
            this.lblAddedString.Location = new System.Drawing.Point(12, 170);
            this.lblAddedString.Name = "lblAddedString";
            this.lblAddedString.Size = new System.Drawing.Size(84, 13);
            this.lblAddedString.TabIndex = 11;
            this.lblAddedString.Text = "Chaine a ajouter";
            // 
            // lblTimeBetweenAsking
            // 
            this.lblTimeBetweenAsking.AutoSize = true;
            this.lblTimeBetweenAsking.Location = new System.Drawing.Point(389, 12);
            this.lblTimeBetweenAsking.Name = "lblTimeBetweenAsking";
            this.lblTimeBetweenAsking.Size = new System.Drawing.Size(134, 13);
            this.lblTimeBetweenAsking.TabIndex = 13;
            this.lblTimeBetweenAsking.Text = "Temps entre les demandes";
            // 
            // nudTimeBetweenAsking
            // 
            this.nudTimeBetweenAsking.Location = new System.Drawing.Point(529, 10);
            this.nudTimeBetweenAsking.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudTimeBetweenAsking.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTimeBetweenAsking.Name = "nudTimeBetweenAsking";
            this.nudTimeBetweenAsking.Size = new System.Drawing.Size(57, 20);
            this.nudTimeBetweenAsking.TabIndex = 6;
            this.nudTimeBetweenAsking.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lblParameter
            // 
            this.lblParameter.AutoSize = true;
            this.lblParameter.Location = new System.Drawing.Point(9, 82);
            this.lblParameter.Name = "lblParameter";
            this.lblParameter.Size = new System.Drawing.Size(103, 13);
            this.lblParameter.TabIndex = 10;
            this.lblParameter.Text = "Parametre a modifier";
            // 
            // cbxParameter
            // 
            this.cbxParameter.FormattingEnabled = true;
            this.cbxParameter.Location = new System.Drawing.Point(12, 106);
            this.cbxParameter.Name = "cbxParameter";
            this.cbxParameter.Size = new System.Drawing.Size(156, 21);
            this.cbxParameter.TabIndex = 1;
            this.cbxParameter.SelectedIndexChanged += new System.EventHandler(this.ParameterChanged);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(12, 241);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(158, 23);
            this.btnApply.TabIndex = 5;
            this.btnApply.Text = "Appliquer les changements";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.ApplyChanges);
            // 
            // lblSaveFolder
            // 
            this.lblSaveFolder.AutoSize = true;
            this.lblSaveFolder.Location = new System.Drawing.Point(389, 51);
            this.lblSaveFolder.Name = "lblSaveFolder";
            this.lblSaveFolder.Size = new System.Drawing.Size(110, 13);
            this.lblSaveFolder.TabIndex = 14;
            this.lblSaveFolder.Text = "Dossier de sauvgarde";
            // 
            // btnChangeSaveFolder
            // 
            this.btnChangeSaveFolder.Location = new System.Drawing.Point(392, 104);
            this.btnChangeSaveFolder.Name = "btnChangeSaveFolder";
            this.btnChangeSaveFolder.Size = new System.Drawing.Size(75, 23);
            this.btnChangeSaveFolder.TabIndex = 8;
            this.btnChangeSaveFolder.Text = "Changer";
            this.btnChangeSaveFolder.UseVisualStyleBackColor = true;
            this.btnChangeSaveFolder.Click += new System.EventHandler(this.ChangeSaveFolder_Click);
            // 
            // tbxSaveFolder
            // 
            this.tbxSaveFolder.Location = new System.Drawing.Point(392, 75);
            this.tbxSaveFolder.Name = "tbxSaveFolder";
            this.tbxSaveFolder.ReadOnly = true;
            this.tbxSaveFolder.Size = new System.Drawing.Size(156, 20);
            this.tbxSaveFolder.TabIndex = 7;
            // 
            // ConfigurationWindow
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 392);
            this.Controls.Add(this.tbxSaveFolder);
            this.Controls.Add(this.btnChangeSaveFolder);
            this.Controls.Add(this.lblSaveFolder);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.lblParameter);
            this.Controls.Add(this.cbxParameter);
            this.Controls.Add(this.nudTimeBetweenAsking);
            this.Controls.Add(this.lblTimeBetweenAsking);
            this.Controls.Add(this.lblAddedString);
            this.Controls.Add(this.lblChooseCategory);
            this.Controls.Add(this.tbxAddString);
            this.Controls.Add(this.btnAddString);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.lbxStrings);
            this.Controls.Add(this.cbxCategory);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConfigurationWindow";
            this.Text = "ConfigurationWindow";
            this.Load += new System.EventHandler(this.ConfigurationWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeBetweenAsking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxCategory;
        private System.Windows.Forms.ListBox lbxStrings;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAddString;
        private System.Windows.Forms.TextBox tbxAddString;
        private System.Windows.Forms.Label lblChooseCategory;
        private System.Windows.Forms.Label lblAddedString;
        private System.Windows.Forms.Label lblTimeBetweenAsking;
        private System.Windows.Forms.NumericUpDown nudTimeBetweenAsking;
        private System.Windows.Forms.Label lblParameter;
        private System.Windows.Forms.ComboBox cbxParameter;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblSaveFolder;
        private System.Windows.Forms.FolderBrowserDialog fbdSaveFolder;
        private System.Windows.Forms.Button btnChangeSaveFolder;
        private System.Windows.Forms.TextBox tbxSaveFolder;
    }
}