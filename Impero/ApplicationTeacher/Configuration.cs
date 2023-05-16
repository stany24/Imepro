using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using LibraryData;

namespace ApplicationTeacher
{
    /// <summary>
    /// Classe pour tous les paramètres de configuraion
    /// </summary>
    public static class Configuration
    {
        public static Dictionary<string,string> DifferentFocus = new();
        public static List<string> IgnoredProcesses = new();
        public static List<string> AlertedProcesses = new();
        public static List<string> AlertedUrls = new();
        public static List<string> AutorisedWebsite = new();
        public static List<DataForTeacher> StudentToShareScreen = new();
        public static StreamOptions streamoptions;
        public static string pathToSaveFolder = "C:\\ProgramData\\Imepro\\";
        public static string FileNameIgnoredProcesses = "ProcessusIgnore.json";
        public static string FileNameAlertedProcesses = "ProcessusAlerté.json";
        public static string FileNameAlertedUrl = "UrlsAlerté.json";
        public static string FileNameAutorisedWebsite = "SitesAutorise.json";
        public static string FileNameDifferentFocus = "ChoixFocus.txt";
        public static int DurationBetweenDemand = 15;
        public static int DefaultTimeout = 2000;
    }
}
