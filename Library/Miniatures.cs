using IronSoftware.Drawing;
using Point = IronSoftware.Drawing.Point;
using Avalonia.Controls;
using Image = Avalonia.Controls.Image;

namespace Library
{
    /// <summary>
    /// Class for preview: a screenshot with the name of the student.
    /// </summary>
    public class Preview : UserControl
    {
        #region Variables
        
        public int StudentId { get; set; }
        internal Image PbxImage { get; set; }
        private readonly Label lblComputerInformation;
        private readonly Button btnSaveScreenShot;
        private const int MarginBetweenText = 5;
        public int TimeSinceUpdate { get; set; }
        private readonly string ComputerName;
        private readonly string SavePath;

        #endregion

        public string GetComputerName() { return ComputerName; }

        #region Constructor

        /// <summary>
        /// Constructor to create and place the preview.
        /// </summary>
        /// <param name="image">The screenshot of the student.</param>
        /// <param name="name">The computer name.</param>
        /// <param name="studentId">The student ID.</param>
        /// <param name="savePath">The save path for images.</param>
        public Preview(AnyBitmap image, string name, int studentId, string savePath)
        {
            TimeSinceUpdate = 0;
            PbxImage = new Image();
            Size = new Size((int)PbxImage.Source.Size.Width,(int)PbxImage.Source.Size.Height);
            StudentId = studentId;
            ComputerName = name;
            SavePath = savePath;

            PbxImage = new Image()
            {
                Location = new Point(0, 0),
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(400, 100),
            };
            PbxImage.SizeChanged += new EventHandler(UpdatePositionsRelativeToImage);
            PbxImage.chan LocationChanged += new EventHandler(UpdatePositionsRelativeToImage);
            Controls.Add(PbxImage);

            lblComputerInformation = new Label
            {
                Location = new Point(140, 0),
                Size = new Size(100, 20),
                Text = ComputerName + " " + TimeSinceUpdate,
            };
            Controls.Add(lblComputerInformation);

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
            PbxImage.Image.Save(SavePath + "\\" + ComputerName + DateTime.Now.ToString("_yyyy-mm-dd_hh-mm-ss") + ".jpg", AnyBitmap.ImageFormat.Jpeg);
        }

        #endregion

        #region Update

        /// <summary>
        /// Function to update the time under a preview.
        /// </summary>
        public void UpdateTime()
        {
            TimeSinceUpdate++;
            lblComputerInformation.Invoke(new MethodInvoker(delegate { lblComputerInformation.Text = ComputerName + " " + TimeSinceUpdate + "s"; }));
        }

        /// <summary>
        /// Function to move the label relative to the picturebox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePositionsRelativeToImage(object sender, EventArgs e)
        {
            Size = new Size(PbxImage.Width, PbxImage.Height + 3 * MarginBetweenText + lblComputerInformation.Height);
            btnSaveScreenShot.Left = PbxImage.Location.X + PbxImage.Width / 2 + MarginBetweenText / 2;
            btnSaveScreenShot.Top = PbxImage.Location.Y + PbxImage.Height + MarginBetweenText;
            lblComputerInformation.Left = PbxImage.Location.X + PbxImage.Width / 2 - lblComputerInformation.Width - MarginBetweenText / 2;
            lblComputerInformation.Top = btnSaveScreenShot.Location.Y + (btnSaveScreenShot.Height - lblComputerInformation.Height);
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
        private const int Margin = 10;
        public double Zoom { get; set; }

        #endregion

        #region Constructor

        public PreviewDisplayer(int maxWidth)
        {
            Zoom = 0.1;
            CustomPreviewList = new List<Preview>();
            MaxWidth = maxWidth;
            Task.Run(LaunchTimeUpdate);
        }

        #endregion

        #region Teacher Action

        /// <summary>
        /// Function to zoom in and out of the panel.
        /// </summary>
        public void ChangeZoom()
        {
            foreach ((Preview? preview, double newHeight, double newWidth) in from Preview preview in CustomPreviewList
                                                           let newHeight = preview.PbxImage.Image.Height * Zoom
                                                           let newWidth = preview.PbxImage.Image.Width * Zoom
                                                           select (preview, newHeight, newWidth))
            {
                preview.PbxImage.Height = Convert.ToInt32(newHeight);
                preview.PbxImage.Width = Convert.ToInt32(newWidth);
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
        public void UpdateAllLocations(int maxWidth)
        {
            MaxWidth = maxWidth;
            int offsetTop = 0;
            int offsetRight = 0;
            int maxHeightInRow = 0;
            foreach (Preview preview in CustomPreviewList)
            {
                if (offsetRight + preview.Width > MaxWidth)
                {
                    offsetTop += maxHeightInRow;
                    maxHeightInRow = 0;
                    offsetRight = 0;
                }
                preview.Top = offsetTop;
                preview.Left = offsetRight + Margin;
                offsetRight += preview.Width + Margin;
                if (preview.Height > maxHeightInRow) { maxHeightInRow = preview.Height; }
            }
        }

        /// <summary>
        /// Function to update the preview image.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <param name="computerName">The computer name.</param>
        /// <param name="image">The new image.</param>
        public void UpdatePreview(int id, string computerName, AnyBitmap image)
        {
            foreach (Preview preview in CustomPreviewList.Where(preview => preview.StudentId == id && preview.GetComputerName() == computerName))
            {
                preview.PbxImage.Source = image;
                preview.TimeSinceUpdate = 0;
                return;
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
        public void RemovePreview(int id)
        {
            foreach (Preview preview in CustomPreviewList)
            {
                if (preview.StudentId != id) continue;
                CustomPreviewList.Remove(preview);
                preview.Dispose();
                UpdateAllLocations(MaxWidth);
                break;
            }
        }

        #endregion
    }
}
