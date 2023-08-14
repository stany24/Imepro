using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LibraryData
{

    /// <summary>
    /// Class that holds the history of all browser
    /// </summary>
    [Serializable]
    public class HistoriqueUrls
    {
        private string[] AllBrowserName {get;}
        [JsonInclude]
        private Dictionary<string, List<Url>> AllBrowser { get; set; }

        /// <summary>
        /// Function to add a new url
        /// </summary>
        /// <param name="url">the url</param>
        /// <param name="browser">the browser the url comes from</param>
        public void AddUrl(Url url,string browser)
        {

            if (AllBrowser[browser].Count == 0 ) { AllBrowser[browser].Add(url); return; }
            if (AllBrowser[browser].Last().Name == url.Name) { return; }
            AllBrowser[browser].Add(url);
        }

        public HistoriqueUrls()
        {
            AllBrowser = new();
            AllBrowserName = new string[9] { "chrome", "firefox", "seleniumchrome", "seleniumfirefox", "opera", "msedge", "safari", "iexplorer", "custom" };
            for (int i = 0;i < AllBrowserName.Count(); i++) { AllBrowser.Add(AllBrowserName[i], new List<Url>()); }
        }
    }

    /// <summary>
    /// Class representing an url
    /// </summary>
    [Serializable]
    public class Url
    {
        [JsonInclude]
        readonly public DateTime CaptureTime;
        [JsonInclude]
        readonly public string Name;

        public Url(DateTime capturetime, string name)
        {
            CaptureTime = capturetime;
            Name = name;
        }

        public override string ToString()
        {
            return CaptureTime.ToString("HH:mm:ss") + " " + Name;
        }
    }
}
