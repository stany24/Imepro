using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibraryData
{
    /// <summary>
    /// Classe qui reprèsente un historique des urls pour tous les navigateurs
    /// </summary>
    [Serializable]
    public class HistoriqueUrls
    {
        [JsonInclude]
        public List<Url> chrome = new();
        [JsonInclude]
        public List<Url> firefox = new();
        [JsonInclude]
        public List<Url> seleniumchrome = new();
        [JsonInclude]
        public List<Url> seleniumfirefox = new();
        [JsonInclude]
        public List<Url> opera = new();
        [JsonInclude]
        public List<Url> edge = new();
        [JsonInclude]
        public List<Url> safari = new();
        [JsonInclude]
        public List<Url> iexplorer = new();
        [JsonInclude]
        public List<Url> custom = new();

        public void AddUrl(Url url)
        {
            switch (url.Browser)
            {
                case "chrome":
                    VerifyUrl(chrome, url);
                    break;
                case "firefox":
                    VerifyUrl(firefox, url);
                    break;
                case "seleniumchrome":
                    VerifyUrl(seleniumchrome, url);
                    break;
                case "seleniumfirefox":
                    VerifyUrl(seleniumfirefox, url);
                    break;
                case "opera":
                    VerifyUrl(opera, url);
                    break;
                case "msedge":
                    VerifyUrl(edge, url);
                    break;
                case "safari":
                    VerifyUrl(safari, url);
                    break;
                case "iexplorer":
                    VerifyUrl(iexplorer, url);
                    break;
                case "custom":
                    VerifyUrl(custom, url);
                    break;
                default: break;
            }
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
