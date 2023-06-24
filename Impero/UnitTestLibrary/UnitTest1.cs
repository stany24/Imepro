using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LibraryData;
namespace UnitTestLibrary
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

        }
    }
}
