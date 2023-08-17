using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace LibraryData
{
    /// <summary>
    /// Class for miniatures: a screenshot with the name of the student.
    /// </summary>
    public class Miniature : UserControl
    {
        #region Variables

        public int StudentID { get; set; }
        internal PictureBox PbxImage { get; set; }
        private readonly Label lblComputerInformations = new();
        private readonly Button btnSaveScreenShot = new();
        readonly int MargeBetweenText = 5;
        public int TimeSinceUpdate { get; set; }
        readonly private string ComputerName;
        private readonly string SavePath;

        #endregion

        public string GetComputerName(){ return ComputerName; }

        #region Constructor

        /// <summary>
        /// Constructor to create and place the miniature.
        /// </summary>
        /// <param name="image">The screenshot of the student.</param>
        /// <param name="name">The computer name.</param>
        /// <param name="studentID">The student ID.</param>
        /// <param name="savepath">The save path for images.</param>
        public Miniature(Bitmap image, string name, int studentID, string savepath)
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
            PbxImage.Image.Save(SavePath + ComputerName + DateTime.Now.ToString("_yyyy-mm-dd_hh-mm-ss") + ".jpg", ImageFormat.Jpeg);
        }

        #endregion

        #region Update

        /// <summary>
        /// Function to update the time under a miniature.
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
            Size = new Size(PbxImage.Width, PbxImage.Height + 3 * MargeBetweenText + lblComputerInformations.Height);
            btnSaveScreenShot.Left = PbxImage.Location.X + PbxImage.Width / 2 + MargeBetweenText / 2;
            btnSaveScreenShot.Top = PbxImage.Location.Y + PbxImage.Height + MargeBetweenText;
            lblComputerInformations.Left = PbxImage.Location.X + PbxImage.Width / 2 - lblComputerInformations.Width - MargeBetweenText / 2;
            lblComputerInformations.Top = btnSaveScreenShot.Location.Y + (btnSaveScreenShot.Height - lblComputerInformations.Height);
        }

        #endregion
    }

    /// <summary>
    /// Class to display multiple miniatures in a panel.
    /// </summary>
    public class MiniatureDisplayer
    {
        #region Variables

        public List<Miniature> MiniatureList { get; set; }
        private int MaxWidth;
        private readonly int Marge = 10;
        public double Zoom { get; set; }

        #endregion

        #region Constructor

        public MiniatureDisplayer(int maxwidth)
        {
            Zoom = 0.1;
            MiniatureList = new();
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
            foreach (var (miniature, NewHeight, NewWidth) in from Miniature miniature in MiniatureList
                                                             let NewHeight = miniature.PbxImage.Image.Height * Zoom
                                                             let NewWidth = miniature.PbxImage.Image.Width * Zoom
                                                             select (miniature, NewHeight, NewWidth))
            {
                miniature.PbxImage.Height = Convert.ToInt32(NewHeight);
                miniature.PbxImage.Width = Convert.ToInt32(NewWidth);
            }

            UpdateAllLocations(MaxWidth);
        }

        #endregion

        #region Update

        /// <summary>
        /// Function to update all miniature time.
        /// </summary>
        private void LaunchTimeUpdate()
        {
            Thread.Sleep(3000);
            while (true)
            {
                Thread.Sleep(1000);
                foreach (Miniature miniature in MiniatureList)
                {
                    miniature.UpdateTime();
                }
            }
        }

        /// <summary>
        /// Function to place all miniatures at the right place.
        /// </summary>
        public void UpdateAllLocations(int maxwidth)
        {
            MaxWidth = maxwidth;
            int OffsetTop = 0;
            int OffsetRight = 0;
            int MaxHeightInRow = 0;
            for (int i = 0; i < MiniatureList.Count; i++)
            {
                if (OffsetRight + MiniatureList[i].Width > MaxWidth)
                {
                    OffsetTop += MaxHeightInRow;
                    MaxHeightInRow = 0;
                    OffsetRight = 0;
                }
                MiniatureList[i].Top = OffsetTop;
                MiniatureList[i].Left = OffsetRight + Marge;
                OffsetRight += MiniatureList[i].Width + Marge;
                if (MiniatureList[i].Height > MaxHeightInRow) { MaxHeightInRow = MiniatureList[i].Height; }
            }
        }

        /// <summary>
        /// Function to update the miniature image.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <param name="computername">The computer name.</param>
        /// <param name="image">The new image.</param>
        public void UpdateMiniature(int id, string computername, Bitmap image)
        {
            foreach (Miniature miniature in MiniatureList)
            {
                if (miniature.StudentID == id && miniature.GetComputerName() == computername)
                {
                    miniature.PbxImage.Image = image;
                    miniature.TimeSinceUpdate = 0;
                    return;
                }
            }
        }

        #endregion

        #region Getter / Setter

        /// <summary>
        /// Function to add a new miniature to the panel.
        /// </summary>
        /// <param name="miniature"></param>
        public void AddMiniature(Miniature miniature)
        {
            MiniatureList.Add(miniature);
            ChangeZoom();
        }

        /// <summary>
        /// Function to remove a miniature of the panel.
        /// </summary>
        /// <param name="id">The student ID.</param>
        /// <param name="computername">The computer name.</param>
        public void RemoveMiniature(int id)
        {
            foreach (Miniature miniature in MiniatureList)
            {
                if (miniature.StudentID == id)
                {
                    MiniatureList.Remove(miniature);
                    miniature.Dispose();
                    UpdateAllLocations(MaxWidth);
                    break;
                }
            }
        }

        #endregion
    }
}
