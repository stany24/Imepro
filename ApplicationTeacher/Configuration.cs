using LibraryData;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ApplicationTeacher
{
    public static class Configuration
    {
        public static StreamOptions GetStreamOptions()
        { return JsonSerializer.Deserialize<StreamOptions>(Properties.Settings.Default.StreamOptions); }

        public static void SetStreamOptions(StreamOptions streamOptions)
        { Properties.Settings.Default.StreamOptions = JsonSerializer.Serialize(streamOptions); }

        public static Dictionary<string, List<string>> GetAllFocus()
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
        {
            try { return Properties.Settings.Default.IgnoredProcesses.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        public static void SetIgnoredProcesses(List<string> ignoredProcesses)
        {
            if (Properties.Settings.Default.IgnoredProcesses == null) { Properties.Settings.Default.IgnoredProcesses = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.IgnoredProcesses.Clear();
            Properties.Settings.Default.IgnoredProcesses.AddRange(ignoredProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        public static List<string> GetIgnoredUrls()
        {
            try { return Properties.Settings.Default.IgnoredUrls.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        public static void SetIgnoredUrls(List<string> ignoredProcesses)
        {
            if (Properties.Settings.Default.IgnoredUrls == null) { Properties.Settings.Default.IgnoredUrls = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.IgnoredUrls.Clear();
            Properties.Settings.Default.IgnoredUrls.AddRange(ignoredProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        public static List<string> GetAlertedUrls()
        {
            try { return Properties.Settings.Default.AlertedUrls.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        public static void SetAlertedUrls(List<string> alertedUrls)
        {
            if (Properties.Settings.Default.AlertedUrls == null) { Properties.Settings.Default.AlertedUrls = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.AlertedUrls.Clear();
            Properties.Settings.Default.AlertedUrls.AddRange(alertedUrls.ToArray());
            Properties.Settings.Default.Save();
        }
        public static List<string> GetAlertedProcesses()
        {
            try { return Properties.Settings.Default.AlertedProcesses.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        public static void SetAlertedProcesses(List<string> alertedProcesses)
        {
            if (Properties.Settings.Default.AlertedProcesses == null) { Properties.Settings.Default.AlertedProcesses = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.AlertedProcesses.Clear();
            Properties.Settings.Default.AlertedProcesses.AddRange(alertedProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        public static List<string> GetAutorisedWebsite()
        {
            try { return Properties.Settings.Default.AutorisedWebsite.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        public static void SetAutorisedWebsite(List<string> autorisedwWbsites)
        {
            if (Properties.Settings.Default.AutorisedWebsite == null) { Properties.Settings.Default.AutorisedWebsite = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.AutorisedWebsite.Clear();
            Properties.Settings.Default.AutorisedWebsite.AddRange(autorisedwWbsites.ToArray());
            Properties.Settings.Default.Save();
        }

        public static bool GetFilterEnabled()
        { return Properties.Settings.Default.FilterEnabled; }

        public static void SetFilterEnabled(bool isEnabled)
        {
            Properties.Settings.Default.FilterEnabled = isEnabled;
            Properties.Settings.Default.Save();
        }
    }
}
