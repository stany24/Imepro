using LibraryData;
using System.Collections.Generic;

namespace ApplicationTeacher
{
    public class ConfigurationDynamique
    {
        public Dictionary<string,List<string>> AllLists = new();

        public ConfigurationDynamique()
        {
            AllLists.Add("Processus ignorés", ConfigurationStatic.IgnoredProcesses);
            AllLists.Add("Urls alertés", ConfigurationStatic.AlertedUrls);
            AllLists.Add("Processus alertés", ConfigurationStatic.AlertedProcesses);
            AllLists.Add("Urls autorisés", ConfigurationStatic.AutorisedWebsite);
        }
    }
    /// <summary>
    /// Classe pour tous les paramètres de configuraion
    /// </summary>
    public static class ConfigurationStatic
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
        public static int ScreenToShareIndex;
    }
}
