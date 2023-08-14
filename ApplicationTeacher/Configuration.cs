using LibraryData;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ApplicationTeacher
{
    public static class Configuration
    {
        public static List<DataForTeacher> GetStudentToShareScreen()
        { return JsonSerializer.Deserialize<List<DataForTeacher>>(Properties.Settings.Default.StudentToShareScreen); }

        public static void SetStudentToShareScreen(List<DataForTeacher> studentToShare)
        {Properties.Settings.Default.StudentToShareScreen = JsonSerializer.Serialize(studentToShare);}

        public static StreamOptions GetStreamOptions()
        { return JsonSerializer.Deserialize<StreamOptions>(Properties.Settings.Default.StreamOptions); }

        public static void SetStreamOptions(StreamOptions streamOptions)
        { Properties.Settings.Default.StreamOptions = JsonSerializer.Serialize(streamOptions); }

        public static Dictionary<string,List<string>> GetAllFocus()
        {
            try { return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(Properties.Settings.Default.AllFocus); }
            catch { return new Dictionary<string, List<string>>(); }
        }

        public static void SetAllFocus(Dictionary<string, List<string>> allFocus)
        {
            Properties.Settings.Default.AllFocus = JsonSerializer.Serialize(allFocus);
            Properties.Settings.Default.Save();
        }

        public static List<string> GetIgnoredProcesses()
        { return Properties.Settings.Default.IgnoredProcesses.Cast<string>().ToList();}

        public static void SetIgnoredProcesses(List<string> ignoredProcesses)
        {
            Properties.Settings.Default.IgnoredProcesses.Clear();
            Properties.Settings.Default.IgnoredProcesses.AddRange(ignoredProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        public static List<string> GetAlertedUrls()
        { return Properties.Settings.Default.AlertedUrls.Cast<string>().ToList(); }

        public static void SetAlertedUrls(List<string> alertedUrls)
        {
            Properties.Settings.Default.AlertedUrls.Clear();
            Properties.Settings.Default.AlertedUrls.AddRange(alertedUrls.ToArray());
            Properties.Settings.Default.Save();
        }
        public static List<string> GetAlertedProcesses()
        { return Properties.Settings.Default.AlertedProcesses.Cast<string>().ToList(); }

        public static void SetAlertedProcesses(List<string> alertedProcesses)
        {
            Properties.Settings.Default.AlertedProcesses.Clear();
            Properties.Settings.Default.AlertedProcesses.AddRange(alertedProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        public static List<string> GetAutorisedWebsite()
        { return Properties.Settings.Default.AutorisedWebsite.Cast<string>().ToList(); }

        public static void SetAutorisedWebsite(List<string> autorisedwWbsites)
        {
            Properties.Settings.Default.AutorisedWebsite.Clear();
            Properties.Settings.Default.AutorisedWebsite.AddRange(autorisedwWbsites.ToArray());
            Properties.Settings.Default.Save();
        }
    }
}
