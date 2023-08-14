﻿using System;
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
    /// Class pour les miniatures: une capture d'écran avec en desous le nom du poste
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
        internal string ComputerName { get; set; }
        private readonly string SavePath;

        #endregion

        #region Constructor

        /// <summary>
        /// Consctucteur pour créer et positionner un miniature
        /// </summary>
        /// <param name="image">L'image à afficher</param>
        /// <param name="name">Le nom de l'ordinateur</param>
        /// <param name="studentID">L'id de l'élève</param>
        /// <param name="savepath">Le chemin de sauvegarde des images</param>
        public Miniature(Bitmap image, string name, int studentID, string savepath)
        {
            TimeSinceUpdate = 0;
            PbxImage = new();

            //valeurs pour la fenêtre de control
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
        /// Function pour sauvegarder la capture d'écran actuel
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
        /// Function qui ajoute une seconde au temps depuis la mise à jour de l'image et change le texte du label.
        /// </summary>
        public void UpdateTime()
        {
            TimeSinceUpdate++;
            lblComputerInformations.Invoke(new MethodInvoker(delegate { lblComputerInformations.Text = ComputerName + " " + TimeSinceUpdate + "s"; }));
        }

        /// <summary>
        /// Function qui positionne le label par rapport à la picturebox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdatePositionsRelativeToImage(object sender, EventArgs e)
        {
            //taille de la picturebox
            Size = new Size(PbxImage.Width, PbxImage.Height + 3 * MargeBetweenText + lblComputerInformations.Height);
            //postion du bouton
            btnSaveScreenShot.Left = PbxImage.Location.X + PbxImage.Width / 2 + MargeBetweenText / 2;
            btnSaveScreenShot.Top = PbxImage.Location.Y + PbxImage.Height + MargeBetweenText;
            //position du label
            lblComputerInformations.Left = PbxImage.Location.X + PbxImage.Width / 2 - lblComputerInformations.Width - MargeBetweenText / 2;
            lblComputerInformations.Top = btnSaveScreenShot.Location.Y + (btnSaveScreenShot.Height - lblComputerInformations.Height);
        }

        #endregion
    }

    /// <summary>
    /// Class pour Afficher plusieurs miniatures dans un panel
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
        /// Function qui permet de zoomer dans les miniatures en changant leur taille
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
        /// Function qui toutes les seconde lance une mise à jour du temps
        /// </summary>
        private void LaunchTimeUpdate()
        {
            Thread.Sleep(3000);
            while (true)
            {
                Thread.Sleep(1000);
                Task.Run(UpdateAllTimes);
            }
        }

        /// <summary>
        /// Function qui lance la mise à jour du temps dans toutes les miniatures
        /// </summary>
        private void UpdateAllTimes()
        {
            foreach (Miniature miniature in MiniatureList)
            {
                miniature.UpdateTime();
            }
        }

        /// <summary>
        /// Function qui place toutes les miniatures au bon endroit
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
        /// Function pour mettre à jour l'image d'une miniature
        /// </summary>
        /// <param name="id">Id de l'élève</param>
        /// <param name="computername"> Nom de l'ordinateur</param>
        /// <param name="image">La nouvelle image que l'on veux mettre</param>
        public void UpdateMiniature(int id, string computername, Bitmap image)
        {
            foreach (Miniature miniature in MiniatureList)
            {
                if (miniature.StudentID == id && miniature.ComputerName == computername)
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
        /// Function pour ajouter une miniature que le miniatureDisplayer doit gérer
        /// </summary>
        /// <param name="miniature"></param>
        public void AddMiniature(Miniature miniature)
        {
            MiniatureList.Add(miniature);
            ChangeZoom();
        }

        /// <summary>
        /// Function pour enlever un miniature de la liste que le miniatureDisplayer doit gérer
        /// </summary>
        /// <param name="id">Id de l'éléve</param>
        /// <param name="computername">Le nom de l'ordinateur</param>
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
