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
            this.lbxStringsList = new System.Windows.Forms.ListBox();
            this.btnRemoveStringList = new System.Windows.Forms.Button();
            this.btnAddStringList = new System.Windows.Forms.Button();
            this.tbxAddStringList = new System.Windows.Forms.TextBox();
            this.lblAddedString = new System.Windows.Forms.Label();
            this.lblTimeBetweenAsking = new System.Windows.Forms.Label();
            this.nudTimeBetweenAsking = new System.Windows.Forms.NumericUpDown();
            this.lblListes = new System.Windows.Forms.Label();
            this.cbxSelectList = new System.Windows.Forms.ComboBox();
            this.lblSaveFolder = new System.Windows.Forms.Label();
            this.fbdSaveFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btnChangeSaveFolder = new System.Windows.Forms.Button();
            this.tbxSaveFolder = new System.Windows.Forms.TextBox();
            this.btnNewFocus = new System.Windows.Forms.Button();
            this.lblFocus = new System.Windows.Forms.Label();
            this.cbxSelectFocus = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxAddStringFocus = new System.Windows.Forms.TextBox();
            this.btnAddStringFocus = new System.Windows.Forms.Button();
            this.btnRemoveStringFocus = new System.Windows.Forms.Button();
            this.lbxStringsFocus = new System.Windows.Forms.ListBox();
            this.lblFocusName = new System.Windows.Forms.Label();
            this.tbxFocusName = new System.Windows.Forms.TextBox();
            this.lblOther = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeBetweenAsking)).BeginInit();
            this.SuspendLayout();
            // 
            // lbxStringsList
            // 
            this.lbxStringsList.FormattingEnabled = true;
            this.lbxStringsList.Location = new System.Drawing.Point(12, 145);
            this.lbxStringsList.Name = "lbxStringsList";
            this.lbxStringsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxStringsList.Size = new System.Drawing.Size(199, 368);
            this.lbxStringsList.TabIndex = 12;
            // 
            // btnRemoveStringList
            // 
            this.btnRemoveStringList.Location = new System.Drawing.Point(119, 116);
            this.btnRemoveStringList.Name = "btnRemoveStringList";
            this.btnRemoveStringList.Size = new System.Drawing.Size(92, 23);
            this.btnRemoveStringList.TabIndex = 4;
            this.btnRemoveStringList.Text = "Retirer";
            this.btnRemoveStringList.UseVisualStyleBackColor = true;
            this.btnRemoveStringList.Click += new System.EventHandler(this.RemoveSelectedString_Click);
            // 
            // btnAddStringList
            // 
            this.btnAddStringList.Location = new System.Drawing.Point(12, 116);
            this.btnAddStringList.Name = "btnAddStringList";
            this.btnAddStringList.Size = new System.Drawing.Size(101, 23);
            this.btnAddStringList.TabIndex = 3;
            this.btnAddStringList.Text = "Ajouter";
            this.btnAddStringList.UseVisualStyleBackColor = true;
            this.btnAddStringList.Click += new System.EventHandler(this.AddString_Click);
            // 
            // tbxAddStringList
            // 
            this.tbxAddStringList.Location = new System.Drawing.Point(12, 90);
            this.tbxAddStringList.Name = "tbxAddStringList";
            this.tbxAddStringList.Size = new System.Drawing.Size(199, 20);
            this.tbxAddStringList.TabIndex = 2;
            // 
            // lblAddedString
            // 
            this.lblAddedString.AutoSize = true;
            this.lblAddedString.Location = new System.Drawing.Point(10, 74);
            this.lblAddedString.Name = "lblAddedString";
            this.lblAddedString.Size = new System.Drawing.Size(84, 13);
            this.lblAddedString.TabIndex = 11;
            this.lblAddedString.Text = "Chaine a ajouter";
            // 
            // lblTimeBetweenAsking
            // 
            this.lblTimeBetweenAsking.AutoSize = true;
            this.lblTimeBetweenAsking.Location = new System.Drawing.Point(478, 45);
            this.lblTimeBetweenAsking.Name = "lblTimeBetweenAsking";
            this.lblTimeBetweenAsking.Size = new System.Drawing.Size(134, 13);
            this.lblTimeBetweenAsking.TabIndex = 13;
            this.lblTimeBetweenAsking.Text = "Temps entre les demandes";
            // 
            // nudTimeBetweenAsking
            // 
            this.nudTimeBetweenAsking.Location = new System.Drawing.Point(618, 43);
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
            this.nudTimeBetweenAsking.ValueChanged += new System.EventHandler(this.ChangeTimeBetweenAsking);
            // 
            // lblListes
            // 
            this.lblListes.AutoSize = true;
            this.lblListes.Location = new System.Drawing.Point(90, 9);
            this.lblListes.Name = "lblListes";
            this.lblListes.Size = new System.Drawing.Size(34, 13);
            this.lblListes.TabIndex = 10;
            this.lblListes.Text = "Listes";
            // 
            // cbxSelectList
            // 
            this.cbxSelectList.FormattingEnabled = true;
            this.cbxSelectList.Items.AddRange(new object[] {
            "IgnoredProcesses",
            "AlertedProcesses",
            "AlertedUrls",
            "AutorisedWebsite"});
            this.cbxSelectList.Location = new System.Drawing.Point(12, 43);
            this.cbxSelectList.Name = "cbxSelectList";
            this.cbxSelectList.Size = new System.Drawing.Size(199, 21);
            this.cbxSelectList.TabIndex = 1;
            this.cbxSelectList.SelectedIndexChanged += new System.EventHandler(this.ParameterChanged);
            // 
            // lblSaveFolder
            // 
            this.lblSaveFolder.AutoSize = true;
            this.lblSaveFolder.Location = new System.Drawing.Point(478, 84);
            this.lblSaveFolder.Name = "lblSaveFolder";
            this.lblSaveFolder.Size = new System.Drawing.Size(110, 13);
            this.lblSaveFolder.TabIndex = 14;
            this.lblSaveFolder.Text = "Dossier de sauvgarde";
            // 
            // btnChangeSaveFolder
            // 
            this.btnChangeSaveFolder.Location = new System.Drawing.Point(481, 137);
            this.btnChangeSaveFolder.Name = "btnChangeSaveFolder";
            this.btnChangeSaveFolder.Size = new System.Drawing.Size(75, 23);
            this.btnChangeSaveFolder.TabIndex = 8;
            this.btnChangeSaveFolder.Text = "Changer";
            this.btnChangeSaveFolder.UseVisualStyleBackColor = true;
            this.btnChangeSaveFolder.Click += new System.EventHandler(this.ChangeSaveFolder_Click);
            // 
            // tbxSaveFolder
            // 
            this.tbxSaveFolder.Location = new System.Drawing.Point(481, 108);
            this.tbxSaveFolder.Name = "tbxSaveFolder";
            this.tbxSaveFolder.ReadOnly = true;
            this.tbxSaveFolder.Size = new System.Drawing.Size(156, 20);
            this.tbxSaveFolder.TabIndex = 7;
            // 
            // btnNewFocus
            // 
            this.btnNewFocus.Location = new System.Drawing.Point(301, 493);
            this.btnNewFocus.Name = "btnNewFocus";
            this.btnNewFocus.Size = new System.Drawing.Size(100, 23);
            this.btnNewFocus.TabIndex = 15;
            this.btnNewFocus.Text = "Nouveau focus";
            this.btnNewFocus.UseVisualStyleBackColor = true;
            this.btnNewFocus.Click += new System.EventHandler(this.NewFocus_Click);
            // 
            // lblFocus
            // 
            this.lblFocus.AutoSize = true;
            this.lblFocus.Location = new System.Drawing.Point(329, 9);
            this.lblFocus.Name = "lblFocus";
            this.lblFocus.Size = new System.Drawing.Size(36, 13);
            this.lblFocus.TabIndex = 17;
            this.lblFocus.Text = "Focus";
            // 
            // cbxSelectFocus
            // 
            this.cbxSelectFocus.FormattingEnabled = true;
            this.cbxSelectFocus.Location = new System.Drawing.Point(251, 43);
            this.cbxSelectFocus.Name = "cbxSelectFocus";
            this.cbxSelectFocus.Size = new System.Drawing.Size(199, 21);
            this.cbxSelectFocus.TabIndex = 16;
            this.cbxSelectFocus.SelectedIndexChanged += new System.EventHandler(this.SelectedFocusChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(249, 114);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Chaine a ajouter";
            // 
            // tbxAddStringFocus
            // 
            this.tbxAddStringFocus.Location = new System.Drawing.Point(251, 130);
            this.tbxAddStringFocus.Name = "tbxAddStringFocus";
            this.tbxAddStringFocus.Size = new System.Drawing.Size(199, 20);
            this.tbxAddStringFocus.TabIndex = 18;
            // 
            // btnAddStringFocus
            // 
            this.btnAddStringFocus.Location = new System.Drawing.Point(251, 156);
            this.btnAddStringFocus.Name = "btnAddStringFocus";
            this.btnAddStringFocus.Size = new System.Drawing.Size(101, 23);
            this.btnAddStringFocus.TabIndex = 19;
            this.btnAddStringFocus.Text = "Ajouter";
            this.btnAddStringFocus.UseVisualStyleBackColor = true;
            this.btnAddStringFocus.Click += new System.EventHandler(this.AddStringToFocus);
            // 
            // btnRemoveStringFocus
            // 
            this.btnRemoveStringFocus.Location = new System.Drawing.Point(358, 156);
            this.btnRemoveStringFocus.Name = "btnRemoveStringFocus";
            this.btnRemoveStringFocus.Size = new System.Drawing.Size(92, 23);
            this.btnRemoveStringFocus.TabIndex = 20;
            this.btnRemoveStringFocus.Text = "Retirer";
            this.btnRemoveStringFocus.UseVisualStyleBackColor = true;
            this.btnRemoveStringFocus.Click += new System.EventHandler(this.RemoveStringToFocus);
            // 
            // lbxStringsFocus
            // 
            this.lbxStringsFocus.FormattingEnabled = true;
            this.lbxStringsFocus.Location = new System.Drawing.Point(251, 184);
            this.lbxStringsFocus.Name = "lbxStringsFocus";
            this.lbxStringsFocus.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbxStringsFocus.Size = new System.Drawing.Size(199, 303);
            this.lbxStringsFocus.TabIndex = 22;
            // 
            // lblFocusName
            // 
            this.lblFocusName.AutoSize = true;
            this.lblFocusName.Location = new System.Drawing.Point(250, 74);
            this.lblFocusName.Name = "lblFocusName";
            this.lblFocusName.Size = new System.Drawing.Size(74, 13);
            this.lblFocusName.TabIndex = 24;
            this.lblFocusName.Text = "Nouveau nom";
            // 
            // tbxFocusName
            // 
            this.tbxFocusName.Location = new System.Drawing.Point(252, 90);
            this.tbxFocusName.Name = "tbxFocusName";
            this.tbxFocusName.Size = new System.Drawing.Size(125, 20);
            this.tbxFocusName.TabIndex = 23;
            // 
            // lblOther
            // 
            this.lblOther.AutoSize = true;
            this.lblOther.Location = new System.Drawing.Point(551, 9);
            this.lblOther.Name = "lblOther";
            this.lblOther.Size = new System.Drawing.Size(37, 13);
            this.lblOther.TabIndex = 25;
            this.lblOther.Text = "Autres";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(383, 90);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(67, 23);
            this.button1.TabIndex = 26;
            this.button1.Text = "Changer";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.FocusNameChanged);
            // 
            // ConfigurationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 525);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblOther);
            this.Controls.Add(this.lblFocusName);
            this.Controls.Add(this.tbxFocusName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbxAddStringFocus);
            this.Controls.Add(this.btnAddStringFocus);
            this.Controls.Add(this.btnRemoveStringFocus);
            this.Controls.Add(this.lbxStringsFocus);
            this.Controls.Add(this.lblFocus);
            this.Controls.Add(this.cbxSelectFocus);
            this.Controls.Add(this.btnNewFocus);
            this.Controls.Add(this.tbxSaveFolder);
            this.Controls.Add(this.btnChangeSaveFolder);
            this.Controls.Add(this.lblSaveFolder);
            this.Controls.Add(this.lblListes);
            this.Controls.Add(this.cbxSelectList);
            this.Controls.Add(this.nudTimeBetweenAsking);
            this.Controls.Add(this.lblTimeBetweenAsking);
            this.Controls.Add(this.lblAddedString);
            this.Controls.Add(this.tbxAddStringList);
            this.Controls.Add(this.btnAddStringList);
            this.Controls.Add(this.btnRemoveStringList);
            this.Controls.Add(this.lbxStringsList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConfigurationWindow";
            this.Text = "ConfigurationWindow";
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeBetweenAsking)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox lbxStringsList;
        private System.Windows.Forms.Button btnRemoveStringList;
        private System.Windows.Forms.Button btnAddStringList;
        private System.Windows.Forms.TextBox tbxAddStringList;
        private System.Windows.Forms.Label lblAddedString;
        private System.Windows.Forms.Label lblTimeBetweenAsking;
        private System.Windows.Forms.NumericUpDown nudTimeBetweenAsking;
        private System.Windows.Forms.Label lblListes;
        private System.Windows.Forms.ComboBox cbxSelectList;
        private System.Windows.Forms.Label lblSaveFolder;
        private System.Windows.Forms.FolderBrowserDialog fbdSaveFolder;
        private System.Windows.Forms.Button btnChangeSaveFolder;
        private System.Windows.Forms.TextBox tbxSaveFolder;
        private System.Windows.Forms.Button btnNewFocus;
        private System.Windows.Forms.Label lblFocus;
        private System.Windows.Forms.ComboBox cbxSelectFocus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxAddStringFocus;
        private System.Windows.Forms.Button btnAddStringFocus;
        private System.Windows.Forms.Button btnRemoveStringFocus;
        private System.Windows.Forms.ListBox lbxStringsFocus;
        private System.Windows.Forms.Label lblFocusName;
        private System.Windows.Forms.TextBox tbxFocusName;
        private System.Windows.Forms.Label lblOther;
        private System.Windows.Forms.Button button1;
    }
}