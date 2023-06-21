using LibraryData;
using System.Collections.Generic;

namespace ApplicationTeacher
{
    public class ConfigurationDynamique
    {
        public Dictionary<string,List<string>> AllLists = new();
        public Dictionary<string, List<string>> AllFocus = new();

        public ConfigurationDynamique()
        {
            AllLists.Add("Processus ignorés", Properties.Settings.Default.IgnoredProcesses);
            AllLists.Add("Urls alertés", Properties.Settings.Default.AlertedUrls);
            AllLists.Add("Processus alertés", Properties.Settings.Default.AlertedProcesses);
            AllLists.Add("Urls autorisés", Properties.Settings.Default.AutorisedWebsite);
        }
    }
    /// <summary>
    /// Classe pour tous les paramètres de configuraion
    /// </summary>
    public static class ConfigurationStatic
    {
        public static Dictionary<string,List<string>> DifferentFocus = new();
        public static List<DataForTeacher> StudentToShareScreen = new();
        public static StreamOptions streamoptions;
    }
}
