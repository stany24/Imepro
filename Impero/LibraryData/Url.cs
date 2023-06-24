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
        [JsonInclude]
        public Dictionary<string, List<Url>> AllBrowser = new();

        public void AddUrl(Url url)
        {
            VerifyUrl(AllBrowser[url.Browser], url);
        }

        public HistoriqueUrls()
        {
            AllBrowser.Add("chrome", new List<Url>());
            AllBrowser.Add("firefox", new List<Url>());
            AllBrowser.Add("seleniumchrome", new List<Url>());
            AllBrowser.Add("seleniumfirefox", new List<Url>());
            AllBrowser.Add("opera", new List<Url>());
            AllBrowser.Add("msedge", new List<Url>());
            AllBrowser.Add("safari", new List<Url>());
            AllBrowser.Add("iexplorer", new List<Url>());
            AllBrowser.Add("custom", new List<Url>());
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
