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

        public void AddUrl(Url url)
        {
            VerifyUrl(AllBrowser[url.Browser], url);
        }

        public HistoriqueUrls()
        {
            for (int i = 0;i < AllBrowserName.Count(); i++) { AllBrowser.Add(AllBrowserName[i], new List<Url>()); }
        }

        /// <summary>
        /// Fonction qui vérifie si l'url que l'on donne n'est pas déja le dernier de la list
        /// </summary>
        /// <param name="list">la list d'url</param>
        /// <param name="url">le nouvelle url</param>
        private void VerifyUrl(List<Url> list, Url url)
        {
            if (list.Count == 0) { list.Add(url); return; }
            if (list.Last().Name != url.Name) { list.Add(url); return; }
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
        readonly public string Browser;
        [JsonInclude]
        readonly public string Name;

        public Url(DateTime capturetime, string browser, string name)
        {
            CaptureTime = capturetime;
            Browser = browser;
            Name = name;
        }
        public override string ToString()
        {
            return CaptureTime.ToString("HH:mm:ss") + " " + Name;
        }
    }
}
