using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LibraryData;
using System.Linq;
using System.Collections.Generic;

namespace UnitTestClassLibrary
{
    [TestClass]
    public class UnitTestUrl
    {
        [TestMethod]
        public void UrlConstructor()
        {
            string TestStringUrl = "https://this.url";
            DateTime TestDateTime = DateTime.Now;
            Url TestUrl = new Url(TestDateTime,TestStringUrl);
            Assert.AreEqual(TestUrl.CaptureTime, TestDateTime);
            Assert.AreEqual(TestUrl.Name, TestStringUrl);
        }
    }
    [TestClass]
    public class UnitTestHistoriqueUrl
    {
        [TestMethod]
        public void HistoriqueUrlConstructor()
        {
            Historique TestHistory= new Historique();
            Assert.AreEqual(TestHistory.AllBrowser.Count,TestHistory.GetAllBrowserNames().Count());
        }

        [TestMethod]
        public void HistoriqueUrlAdding()
        {
            Historique TestHistory = new Historique();
            foreach (KeyValuePair<string, List<Url>> browser in TestHistory.AllBrowser)
            {
                TestHistory.AddUrl(new Url(DateTime.Now, "testurl"),browser.Key);
                Assert.AreEqual(1,TestHistory.AllBrowser[browser.Key].Count);
                TestHistory.AddUrl(new Url(DateTime.Now, "testurl"),browser.Key);
                Assert.AreEqual(1,TestHistory.AllBrowser[browser.Key].Count);
            }
        }
    }

    [TestClass]
    public class UnitTestStreamOptions
    {
        [TestMethod]
        public void StreamOptionsConstructor()
        {
            Priority priority = Priority.Topmost;
            List<string> strings = new List<string> { "test1", "test2", "test3" };
            StreamOptions options = new StreamOptions(priority,strings);
            Assert.AreEqual(options.GetPriority(), priority);
            Assert.AreEqual(options.GetFocus(), strings);
        }
    }
}
