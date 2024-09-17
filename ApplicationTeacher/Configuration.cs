using System.Collections.Generic;
using System.Linq;
using LibraryData;
using System.Text.Json;

namespace ApplicationTeacher
{
    /// <summary>
    /// Class used to acces and serialize the variables of the config.config file safely.
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// Function to load the stream options
        /// </summary>
        /// <returns>The loaded stream options.</returns>
        public static StreamOptions GetStreamOptions()
        { return JsonSerializer.Deserialize<StreamOptions>(Properties.Settings.Default.StreamOptions); }

        /// <summary>
        /// Function to save the stream options only for one session.
        /// </summary>
        /// <param name="streamOptions">The stream options you want to save.</param>
        public static void SetStreamOptions(StreamOptions streamOptions)
        { Properties.Settings.Default.StreamOptions = JsonSerializer.Serialize(streamOptions); }

        /// <summary>
        /// Function to load all different focus.
        /// </summary>
        /// <returns>All the focus.</returns>
        public static Dictionary<string, List<string>> GetAllFocus()
        {
            try { return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(Properties.Settings.Default.AllFocus); }
            catch { return new Dictionary<string, List<string>>(); }
        }

        /// <summary>
        /// Function to save all focus.
        /// </summary>
        /// <param name="allFocus">All the focus you want to save.</param>
        public static void SetAllFocus(Dictionary<string, List<string>> allFocus)
        {
            Properties.Settings.Default.AllFocus = JsonSerializer.Serialize(allFocus);
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Function to load ignored processes.
        /// </summary>
        /// <returns>The list of ignored processes.</returns>
        public static List<string> GetIgnoredProcesses()
        {
            try { return Properties.Settings.Default.IgnoredProcesses.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// Function to save all ignored processes.
        /// </summary>
        /// <param name="ignoredProcesses">The list of ignored processes you want to save.</param>
        public static void SetIgnoredProcesses(List<string> ignoredProcesses)
        {
            if (Properties.Settings.Default.IgnoredProcesses == null) { Properties.Settings.Default.IgnoredProcesses = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.IgnoredProcesses.Clear();
            Properties.Settings.Default.IgnoredProcesses.AddRange(ignoredProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Function to load ignored urls.
        /// </summary>
        /// <returns>The list of ignored urls.</returns>
        public static List<string> GetIgnoredUrls()
        {
            try { return Properties.Settings.Default.IgnoredUrls.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// Function to save all ignored urls.
        /// </summary>
        /// <param name="ignoredProcesses">The list of ignored urls you want to save.</param>
        public static void SetIgnoredUrls(List<string> ignoredProcesses)
        {
            if (Properties.Settings.Default.IgnoredUrls == null) { Properties.Settings.Default.IgnoredUrls = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.IgnoredUrls.Clear();
            Properties.Settings.Default.IgnoredUrls.AddRange(ignoredProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Function to load alerted urls.
        /// </summary>
        /// <returns>The list of alerted urls.</returns>
        public static List<string> GetAlertedUrls()
        {
            try { return Properties.Settings.Default.AlertedUrls.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// Function to save all alerted urls.
        /// </summary>
        /// <param name="alertedUrls">The list of alerted urls you want to save.</param>
        public static void SetAlertedUrls(List<string> alertedUrls)
        {
            if (Properties.Settings.Default.AlertedUrls == null) { Properties.Settings.Default.AlertedUrls = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.AlertedUrls.Clear();
            Properties.Settings.Default.AlertedUrls.AddRange(alertedUrls.ToArray());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Function to load alerted processes.
        /// </summary>
        /// <returns>The list of alerted processes.</returns>
        public static List<string> GetAlertedProcesses()
        {
            try { return Properties.Settings.Default.AlertedProcesses.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// Function to save all alerted processes.
        /// </summary>
        /// <param name="alertedProcesses">The list of alerted processes you want to save.</param>
        public static void SetAlertedProcesses(List<string> alertedProcesses)
        {
            if (Properties.Settings.Default.AlertedProcesses == null) { Properties.Settings.Default.AlertedProcesses = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.AlertedProcesses.Clear();
            Properties.Settings.Default.AlertedProcesses.AddRange(alertedProcesses.ToArray());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Function to load autorised urls.
        /// </summary>
        /// <returns>The list of autorised urls.</returns>
        public static List<string> GetAutorisedWebsite()
        {
            try { return Properties.Settings.Default.AutorisedWebsite.Cast<string>().ToList(); }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// Function to save all autorised urls.
        /// </summary>
        /// <param name="autorisedwWbsites">The list of autorised urls you want to save.</param>
        public static void SetAutorisedWebsite(List<string> autorisedwWbsites)
        {
            if (Properties.Settings.Default.AutorisedWebsite == null) { Properties.Settings.Default.AutorisedWebsite = new System.Collections.Specialized.StringCollection(); }
            Properties.Settings.Default.AutorisedWebsite.Clear();
            Properties.Settings.Default.AutorisedWebsite.AddRange(autorisedwWbsites.ToArray());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Function to load the if the filter are enabled.
        /// </summary>
        /// <returns>A bool representing the state of filters.</returns>
        public static bool GetFilterEnabled()
        { return Properties.Settings.Default.FilterEnabled; }

        /// <summary>
        /// Function to save if filter are enabled
        /// </summary>
        /// <param name="isEnabled">State of the filters.</param>
        public static void SetFilterEnabled(bool isEnabled)
        {
            Properties.Settings.Default.FilterEnabled = isEnabled;
            Properties.Settings.Default.Save();
        }
    }
}
