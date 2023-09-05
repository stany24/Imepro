﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace LibraryData
{

    /// <summary>
    /// Class that holds the history of all browser.
    /// </summary>
    [Serializable]
    public class History
    {
        [JsonIgnore]
        readonly private string[] AllBrowserName = { "chrome", "firefox", "seleniumchrome", "seleniumfirefox", "opera", "msedge", "safari", "iexplorer", "custom" };
        [JsonInclude]
        public Dictionary<string, List<Url>> AllBrowser { get; set; }

        /// <summary>
        /// Function to add a new url.
        /// </summary>
        /// <param name="url">The new url.</param>
        /// <param name="browser">The browser the url comes from.</param>
        public void AddUrl(Url url,string browser)
        {
            if (AllBrowser[browser].Count == 0 ) { AllBrowser[browser].Add(url); return; }
            if (AllBrowser[browser].Last().Name == url.Name) { return; }
            AllBrowser[browser].Add(url);
        }

        public string[] GetAllBrowserNames() { return AllBrowserName; }

        public History()
        {
            AllBrowser = new();
            for (int i = 0;i < AllBrowserName.Count(); i++) { AllBrowser.Add(AllBrowserName[i], new List<Url>()); }
        }
    }

    /// <summary>
    /// Class representing an url.
    /// </summary>
    [Serializable]
    public class Url
    {
        [JsonInclude]
        readonly public DateTime ScreenShotTime;
        [JsonInclude]
        readonly public string Name;

        public Url(DateTime screenShotTime, string name)
        {
            ScreenShotTime = screenShotTime;
            Name = name;
        }

        public override string ToString()
        {
            return ScreenShotTime.ToString("HH:mm:ss") + " " + Name;
        }
    }
}
