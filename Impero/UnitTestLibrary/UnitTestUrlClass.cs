using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LibraryData;
using System.Linq;
using System.Collections.Generic;

namespace UnitTestUrlClass
{
    [TestClass]
    public class UnitTestUrl
    {
        [TestMethod]
        public void UrlConstructor()
        {
            string TestStringUrl = "https://this.url";
            string TestBrowser = "chrome";
            DateTime TestDateTime = DateTime.Now;
            Url TestUrl = new Url(TestDateTime, TestBrowser,TestStringUrl);
            Assert.AreEqual(TestUrl.CaptureTime, TestDateTime);
            Assert.AreEqual(TestUrl.Browser, TestBrowser);
            Assert.AreEqual(TestUrl.Name, TestStringUrl);
        }
    }
    [TestClass]
    public class UnitTestHistoriqueUrl
    {
        [TestMethod]
        public void HistoriqueUrlConstructor()
        {
            HistoriqueUrls TestHistory= new HistoriqueUrls();
            Assert.AreEqual(TestHistory.AllBrowser.Count(),TestHistory.AllBrowserName.Count());
        }

        [TestMethod]
        public void HistoriqueUrlAdding()
        {
            HistoriqueUrls TestHistory = new HistoriqueUrls();
            foreach (KeyValuePair<string, List<Url>> browser in TestHistory.AllBrowser)
            {
                TestHistory.AddUrl(new Url(DateTime.Now, browser.Key, "testurl"));
                Assert.AreEqual(TestHistory.AllBrowser[browser.Key].Count, 1);
                TestHistory.AddUrl(new Url(DateTime.Now, browser.Key, "testurl"));
                Assert.AreEqual(TestHistory.AllBrowser[browser.Key].Count, 1);
            }
        }
    }
}
