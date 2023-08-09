using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LibraryData
{

    /// <summary>
    /// Classe qui reprèsente un historique des urls pour tous les navigateurs
    /// </summary>
    [Serializable]
    public class HistoriqueUrls
    {
        public string[] AllBrowserName = { "chrome","firefox","seleniumchrome","seleniumfirefox", "opera","msedge", "safari", "iexplorer", "custom" };
        [JsonInclude]
        public Dictionary<string, List<Url>> AllBrowser = new();

        public void AddUrl(Url url,string browser)
        {

            if (AllBrowser[browser].Count == 0 ) { AllBrowser[browser].Add(url); return; }
            if (AllBrowser[browser][-1].Name == url.Name) { return; }
            AllBrowser[browser].Add(url);
        }

        public HistoriqueUrls()
        {
            for (int i = 0;i < AllBrowserName.Count(); i++) { AllBrowser.Add(AllBrowserName[i], new List<Url>()); }
        }
    }

    /// <summary>
    /// Classe qui représente un Url
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
