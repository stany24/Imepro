using System.Text.Json.Serialization;

namespace ClassLibrary6.History
{

    /// <summary>
    /// Class that holds the history of all browser.
    /// </summary>
    [Serializable]
    public class History
    {
        [JsonInclude]
        public Dictionary<BrowserName, List<Url>> AllBrowser { get; set; }

        /// <summary>
        /// Function to add a new url.
        /// </summary>
        /// <param name="url">The new url.</param>
        /// <param name="browser">The browser the url comes from.</param>
        public void AddUrl(Url url, BrowserName browser)
        {
            if (AllBrowser[browser].Count == 0) { AllBrowser[browser].Add(url); return; }
            if (AllBrowser[browser].Last().Name == url.Name) { return; }
            AllBrowser[browser].Add(url);
        }

        public History()
        {
            AllBrowser = new Dictionary<BrowserName, List<Url>>();
            foreach (BrowserName name in Enum.GetValues(typeof(BrowserName)))
            {
                AllBrowser.Add(name, new List<Url>());
            }
        }
    }
}
