﻿using LibraryData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace UnitTestClassLibrary
{
    [TestClass]
    public class UrlTest
    {
        [TestMethod]
        public void UrlConstructor()
        {
            string TestStringUrl = "https://this.url";
            DateTime TestDateTime = DateTime.Now;
            Url TestUrl = new Url(TestDateTime, TestStringUrl);
            Assert.AreEqual(TestUrl.ScreenShotTime, TestDateTime);
            Assert.AreEqual(TestUrl.Name, TestStringUrl);
        }
    }
    [TestClass]
    public class HistoriqueTest
    {
        [TestMethod]
        public void HistoriqueUrlAdding()
        {
            History TestHistory = new History();
            foreach (KeyValuePair<BrowserName, List<Url>> browser in TestHistory.AllBrowser)
            {
                TestHistory.AddUrl(new Url(DateTime.Now, "testurl"), browser.Key);
                Assert.AreEqual(1, TestHistory.AllBrowser[browser.Key].Count);
                TestHistory.AddUrl(new Url(DateTime.Now, "testurl"), browser.Key);
                Assert.AreEqual(1, TestHistory.AllBrowser[browser.Key].Count);
            }
        }
    }
}
