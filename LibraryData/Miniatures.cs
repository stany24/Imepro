namespace LibraryData
{
    /// <summary>
    /// Class for preview: a screenshot with the name of the student.
    /// </summary>
    public class Preview : UserControl
    {
        #region Variables

        public int StudentID { get; set; }
        internal PictureBox PbxImage { get; set; }
        private readonly Label lblComputerInformations = new();
        private readonly Button btnSaveScreenShot = new();
        readonly int MarginBetweenText = 5;
        public int TimeSinceUpdate { get; set; }
        readonly private string ComputerName;
        private readonly string SavePath;

        #endregion

        public string GetComputerName() { return ComputerName; }

        #region Constructor

        /// <summary>
        /// Constructor to create and place the preview.
        /// </summary>
        /// <param name="image">The screenshot of the student.</param>
        /// <param name="name">The computer name.</param>
        /// <param name="studentID">The student ID.</param>
        /// <param name="savepath">The save path for images.</param>
        public Preview(Bitmap image, string name, int studentID, string savepath)
        {
            TimeSinceUpdate = 0;
            PbxImage = new();
            Size = PbxImage.Size;
            StudentID = studentID;
            ComputerName = name;
            SavePath = savepath;

            PbxImage = new PictureBox
            {
                Location = new Point(0, 0),
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(400, 100),
            };
            PbxImage.SizeChanged += new EventHandler(UpdatePositionsRelativeToImage);
            PbxImage.LocationChanged += new EventHandler(UpdatePositionsRelativeToImage);
            Controls.Add(PbxImage);

            lblComputerInformations = new Label
            {
                Location = new Point(140, 0),
                Size = new Size(100, 20),
                Text = ComputerName + " " + TimeSinceUpdate,
            };
            Controls.Add(lblComputerInformations);

            btnSaveScreenShot = new Button
            {
                Location = new Point(0, 0),
                Size = new Size(80, 21),
                Text = "Sauvegarder"
            };
            btnSaveScreenShot.Click += new EventHandler(SaveScreenShot);
            Controls.Add(btnSaveScreenShot);
            UpdatePositionsRelativeToImage(new object(), new EventArgs());
        }

        #endregion

        #region Teacher Action

        /// <summary>
        /// Function to save the current screenshot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveScreenShot(object sender, EventArgs e)
        {
            PbxImage.Image.Save(SavePath + "\\" + ComputerName + DateTime.Now.ToString("_yyyy-mm-dd_hh-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        #endregion

        #region Update

        /// <summary>
        /// Function to update the time under a preview.
        /// </summary>
        public void UpdateTime()
        {
            TimeSinceUpdate++;
            lblComputerInformations.Invoke(new MethodInvoker(delegate { lblComputerInformations.Text = ComputerName + " " + TimeSinceUpdate + "s"; }));
        }

        /// <summary>
        /// Function to move the label relative to the picturebox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePositionsRelativeToImage(object sender, EventArgs e)
        {
            Size = new Size(PbxImage.Width, PbxImage.Height + 3 * MarginBetweenText + lblComputerInformations.Height);
            btnSaveScreenShot.Left = PbxImage.Location.X + PbxImage.Width / 2 + MarginBetweenText / 2;
            btnSaveScreenShot.Top = PbxImage.Location.Y + PbxImage.Height + MarginBetweenText;
            lblComputerInformations.Left = PbxImage.Location.X + PbxImage.Width / 2 - lblComputerInformations.Width - MarginBetweenText / 2;
            lblComputerInformations.Top = btnSaveScreenShot.Location.Y + (btnSaveScreenShot.Height - lblComputerInformations.Height);
        }

        #endregion
    }

    /// <summary>
    /// Class to display multiple preview in a panel.
    /// </summary>
    public class PreviewDisplayer
    {
        #region Variables

        public List<Preview> CustomPreviewList { get; set; }
        private int MaxWidth;
        private readonly int Margin = 10;
        public double Zoom { get; set; }

        #endregion

        #region Constructor

        public PreviewDisplayer(int maxwidth)
        {
            Zoom = 0.1;
            CustomPreviewList = new();
            MaxWidth = maxwidth;
            Task.Run(LaunchTimeUpdate);
        }

        #endregion

        #region Teacher Action

        /// <summary>
        /// Function to zoom in and out of the panel.
        /// </summary>
        public void ChangeZoom()
        {
            foreach (var (preview, NewHeight, NewWidth) in from Preview preview in CustomPreviewList
                                                           let NewHeight = preview.PbxImage.Image.Height * Zoom
                                                           let NewWidth = preview.PbxImage.Image.Width * Zoom
                                                           select (preview, NewHeight, NewWidth))
            {
                preview.PbxImage.Height = Convert.ToInt32(NewHeight);
                preview.PbxImage.Width = Convert.ToInt32(NewWidth);
            }

            UpdateAllLocations(MaxWidth);
        }

        #endregion

        #region Update

        /// <summary>
        /// Function to update all preview time.
        /// </summary>
        private void LaunchTimeUpdate()
        {
            Thread.Sleep(3000);
            while (true)
            {
                Thread.Sleep(1000);
                foreach (Preview preview in CustomPreviewList)
                {
                    preview.UpdateTime();
                }
            }
        }

        /// <summary>
        /// Function to place all previews at the right place.
        /// </summary>
        public void UpdateAllLocations(int maxwidth)
        {
            MaxWidth = maxwidth;
            int OffsetTop = 0;
            int OffsetRight = 0;
            int MaxHeightInRow = 0;
            for (int i = 0; i < CustomPreviewList.Count; i++)
            {
                if (OffsetRight + CustomPreviewList[i].Width > MaxWidth)
                {
                    OffsetTop += MaxHeightInRow;
                    MaxHeightInRow = 0;
                    OffsetRight = 0;
                }
                CustomPreviewList[i].Top = OffsetTop;
                CustomPreviewList[i].Left = OffsetRight + Margin;
                OffsetRight += CustomPreviewList[i].Width + Margin;
                if (CustomPreviewList[i].Height > MaxHeightInRow) { MaxHeightInRow = CustomPreviewList[i].Height; }
            }
        }

        /// <summary>
        /// Function to update the preview image.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <param name="computername">The computer name.</param>
        /// <param name="image">The new image.</param>
        public void UpdatePreview(int id, string computername, Bitmap image)
        {
            foreach (Preview preview in CustomPreviewList)
            {
                if (preview.StudentID == id && preview.GetComputerName() == computername)
                {
                    preview.PbxImage.Image = image;
                    preview.TimeSinceUpdate = 0;
                    return;
                }
            }
        }

        #endregion

        #region Getter / Setter

        /// <summary>
        /// Function to add a new preview to the panel.
        /// </summary>
        /// <param name="preview"></param>
        public void AddPreview(Preview preview)
        {
            CustomPreviewList.Add(preview);
            ChangeZoom();
        }

        /// <summary>
        /// Function to remove a preview of the panel.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <param name="computername">The computer name.</param>
        public void RemovePreview(int id)
        {
            foreach (Preview preview in CustomPreviewList)
            {
                if (preview.StudentID == id)
                {
                    CustomPreviewList.Remove(preview);
                    preview.Dispose();
                    UpdateAllLocations(MaxWidth);
                    break;
                }
            }
        }

        #endregion
    }
}
