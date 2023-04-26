﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace LibraryData
{
    public class Data
    {
        [JsonInclude]
        public string UserName = "";
        [JsonInclude]
        public string ComputerName = "";
        [JsonInclude]
        public List<string> Urls = new();
        [JsonInclude]
        public Dictionary<int, string> Processes = new();
        [JsonIgnore]
        public Bitmap ScreenShot;

        public Data(string userName, string computerName, List<string> urls, Dictionary<int, string> processes)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
        }

        public Data() { }

        public override string ToString()
        {
            string retour = UserName + " utilse le pc " + ComputerName + " avec les processus: ";
            foreach (KeyValuePair<int, string> process in Processes)
            {
                retour += process.Value + " ";
            }
            retour += "et les onglets ouverts: ";
            foreach (string url in Urls)
            {
                retour += url + " ; ";
            }
            return retour.Trim();
        }
    }

    public class DataForTeacher : Data
    {
        public Socket SocketToStudent;
        public int ID;
        public int NumberOfFailure;

        public DataForTeacher(Socket socket, int id)
        {
            SocketToStudent = socket;
            ID = id;
        }

        public DataForTeacher(Data data)
        {
            UserName = data.UserName;
            ComputerName = data.ComputerName;
            Urls = data.Urls;
            Processes = data.Processes;
        }
    }

    public class DataForStudent : Data
    {
        public Socket SocketToTeacher;
        public List<string> DefaultProcess = new();
        readonly public List<string> browsersList = new() { "chrome", "firefox", "iexplore", "safari", "opera", "msedge" };

        public DataForStudent()
        {
            GetDefaultProcesses();
            GetNames();
        }

        public void GetNames()
        {
            ComputerName = Environment.MachineName;
            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        public Data ToData()
        {
            return new Data(UserName, ComputerName, Urls, Processes);
        }

        /// <summary>
        /// Fonction qui permet de récuperer le nom de l'onglet actif dans tous les navigateurs ouverts
        /// </summary>
        public void GetCurrentWebTabsName()
        {
            [DllImport("user32.dll")]
            static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            Urls.Clear();
            foreach (string singleBrowser in browsersList)
            {
                Process[] process = Process.GetProcessesByName(singleBrowser);
                if (process.Length > 0)
                {
                    foreach (Process instance in process)
                    {
                        IntPtr hWnd = instance.MainWindowHandle;

                        StringBuilder text = new(GetWindowTextLength(hWnd) + 1);
                        _ = GetWindowText(hWnd, text, text.Capacity);
                        if (text.ToString() != "") { Urls.Add(singleBrowser + " : " + text.ToString()); }
                    }
                }
            }
        }

        /// <summary>
        /// Fonction qui crée une list de processus lancé par l'ordinateur au démarrage de l'application
        /// </summary>
        public void GetDefaultProcesses()
        {
            foreach (Process process in Process.GetProcesses().OrderBy(x => x.ProcessName)) { DefaultProcess.Add(process.ProcessName); }
        }

        /// <summary>
        /// Fonction qui met à jour la list des processus lancé par l'utilisateur
        /// </summary>
        public void GetUserProcesses()
        {
            Processes.Clear();
            List<Process> list = Process.GetProcesses().OrderBy(x => x.ProcessName).ToList();
            foreach (Process process in list)
            {
                if (!DefaultProcess.Contains(process.ProcessName)) { Processes.Add(process.Id, process.ProcessName); }
            }
        }

        /// <summary>
        /// Fonction qui permet de prendre une capture d'écran de tous les écran puis de les recomposer en une seul image
        /// </summary>
        public void TakeScreenShot()
        {
            int TotalWidth = 0;
            int MaxHeight = 0;
            List<Bitmap> images = new();
            //prend une capture d'écran de tout les écran
            foreach (Screen screen in Screen.AllScreens)
            {
                Bitmap Bitmap = new(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format32bppArgb);
                Rectangle ScreenSize = screen.Bounds;
                Graphics.FromImage(Bitmap).CopyFromScreen(ScreenSize.Left, ScreenSize.Top, 0, 0, ScreenSize.Size);
                images.Add(Bitmap);
                TotalWidth += ScreenSize.Width;
                if (ScreenSize.Height > MaxHeight) { MaxHeight = ScreenSize.Height; }
            }

            Bitmap FullImage = new(TotalWidth, MaxHeight, PixelFormat.Format32bppArgb);
            Graphics FullGraphics = Graphics.FromImage(FullImage);

            int offsetLeft = 0;
            //Crée une seul image de toutes les captures d'écran
            foreach (Bitmap image in images)
            {
                FullGraphics.DrawImage(image, new Point(offsetLeft, 0));
                offsetLeft += image.Width;
            }
            //FullImage = (new Bitmap(FullImage, new Size(200,200)));
            ScreenShot = FullImage;
            FullGraphics.Dispose();
        }
    }

    [ServiceContract]
    public interface IScreenShotInterface
    {
        [OperationContract(IsOneWay = true)]
        void SendScreenShot(ScreenShotPart part);
    }

    [DataContract]
    public class ScreenShotPart
    {
        [DataMember]
        public byte[] Part;

        public ScreenShotPart(byte[] part)
        {
            Part = part;
        }
    }

    [Serializable]
    public class IpForTheWeek
    {
        [JsonInclude]
        public string[] lundi = new string[2];
        [JsonInclude]
        public string[] mardi = new string[2];
        [JsonInclude]
        public string[] mercredi = new string[2];
        [JsonInclude]
        public string[] jeudi = new string[2];
        [JsonInclude]
        public string[] vendredi = new string[2];
        [JsonInclude]
        public string[] samedi = new string[2];
        [JsonInclude]
        public string[] dimanche = new string[2];

        public IpForTheWeek(IpForTheWeek copy)
        {
            lundi= copy.lundi;
            mardi= copy.mardi;
            mercredi= copy.mercredi;
            jeudi= copy.jeudi;
            vendredi= copy.vendredi;
            samedi= copy.samedi;
            dimanche= copy.dimanche;
        }

        /// <summary>
        /// Fonction qui enregistre l'ip donnée au bonne endroit, qui dépand du jour et de l'heure de l'action
        /// </summary>
        /// <param name="ip"></param>
        public void SetIp(string ip)
        {
            try{IPAddress.Parse(ip);}
            catch { return; }

            DayOfWeek day = DateTime.Now.DayOfWeek;
            int MatinOuAprèsMidi = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { MatinOuAprèsMidi = 1; }
            switch (day)
            {
                case DayOfWeek.Monday: lundi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Tuesday: mardi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Wednesday: mercredi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Thursday: jeudi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Friday: vendredi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Saturday: samedi[MatinOuAprèsMidi] = ip; break;
                case DayOfWeek.Sunday: dimanche[MatinOuAprèsMidi] = ip; break;
            }
        }

        /// <summary>
        /// Fonction qui retourne la bonne ip en fonction du jour et de l'heure de l'appel
        /// </summary>
        /// <returns></returns>
        public string GetIp()
        {
            DayOfWeek day = DateTime.Now.DayOfWeek;
            int MatinOuAprèsMidi = 0;
            if (DateTime.Now.TimeOfDay > new TimeSpan(12, 35, 0)) { MatinOuAprèsMidi = 1; }
            return day switch
            {
                DayOfWeek.Monday => lundi[MatinOuAprèsMidi],
                DayOfWeek.Tuesday => mardi[MatinOuAprèsMidi],
                DayOfWeek.Wednesday => mercredi[MatinOuAprèsMidi],
                DayOfWeek.Thursday => jeudi[MatinOuAprèsMidi],
                DayOfWeek.Friday => vendredi[MatinOuAprèsMidi],
                DayOfWeek.Saturday => samedi[MatinOuAprèsMidi],
                DayOfWeek.Sunday => dimanche[MatinOuAprèsMidi],
                _ => null,
            };
        }

        public IpForTheWeek()
        {
            lundi[0] = "157.26.227.198";
            lundi[1] = "157.26.227.198";
            mardi[0] = "157.26.227.198";
            mardi[1] = "157.26.227.198";
            mercredi[0] = "157.26.227.198";
            mercredi[1] = "157.26.227.198";
            jeudi[0] = "157.26.227.198";
            jeudi[1] = "157.26.227.198";
            vendredi[0] = "157.26.227.198";
            vendredi[1] = "157.26.227.198";
            samedi[0] = "157.26.227.198";
            samedi[1] = "157.26.227.198";
            dimanche[0] = "157.26.227.198";
            dimanche[1] = "157.26.227.198";
        }
    }
    public class Miniature
    {
        public int StudentID;
        public PictureBox Image = new();
        Label Name = new();
        Label Age = new();
        int Marge = 10;
        double Zoom = 0.1;

        public Miniature(Bitmap image,string name, string age, int studentID)
        {
            Image.Image = image;
            Image.SizeMode = PictureBoxSizeMode.StretchImage;
            Image.Width = Convert.ToInt32(Image.Image.Width*Zoom);
            Image.Height = Convert.ToInt32(Image.Image.Height*Zoom);
            Name.Text = name;
            Age.Text = age;
            StudentID = studentID;
            Image.SizeChanged += new EventHandler(UpdateLabelsPositions);
            Image.LocationChanged += new EventHandler(UpdateLabelsPositions);
            UpdateLabelsPositions(new object(), new EventArgs());
        }

        public void UpdateLabelsPositions(object sender, EventArgs e)
        {
            Image.Width = Convert.ToInt32(Image.Image.Width * Zoom);
            Image.Height = Convert.ToInt32(Image.Image.Height * Zoom);
            Name.Left = Image.Location.X + Image.Width / 2 - Name.Width/2;
            Name.Top = Image.Location.Y + Image.Height + Marge;
            Age.Left = Image.Location.X + Image.Width / 2 - Name.Width/2;
            Age.Top = Image.Location.Y + Image.Height + 2*Marge + Name.Height;
        }
    }

    public class MiniatureDisplayer
    {
        public List<Miniature> MiniatureList;
        Rectangle Area = new Rectangle(770, 12,702, 937);

        public void UpdateAllLocations()
        {
            foreach(Miniature miniature in MiniatureList)
            {
                miniature.Image.Top = Area.Top;
                miniature.Image.Left= Area.Left;
            }
        }

        public void AddMiniature(Miniature miniature)
        {
            MiniatureList.Add(miniature);
            UpdateAllLocations();
        }
    }

}
