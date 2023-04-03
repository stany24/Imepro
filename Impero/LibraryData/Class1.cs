using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

        public DataForTeacher(Socket socket,int id)
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

        public void GetUrls()
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
        public void GetDefaultProcesses()
        {
            foreach (Process process in Process.GetProcesses().OrderBy(x => x.ProcessName)) { DefaultProcess.Add(process.ProcessName); }
        }

        public void GetUserProcesses()
        {
            Processes.Clear();
            List<Process> list = Process.GetProcesses().OrderBy(x => x.ProcessName).ToList();
            foreach (Process process in list)
            {
                if (!DefaultProcess.Contains(process.ProcessName)) { Processes.Add(process.Id, process.ProcessName); }
            }
        }

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
}
