using ApplicationTeacher;
using LibraryData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestLibrary.Teacher
{
    [TestClass]
    public class ConfigurationTest
    {
        private readonly List<string> list = new List<string>() { "test", "nope", "more", "csgo" };
        [TestMethod]
        public void SetGetAutorisedUrl()
        {
            Configuration.SetAutorisedWebsite(list);
            Assert.AreEqual(list.Count, Configuration.GetAutorisedWebsite().Count);
            for (int i = 0; i < list.Count; i++) { Assert.AreEqual(list[i], Configuration.GetAutorisedWebsite()[i]); }
        }

        [TestMethod]
        public void SetGetAlertedProcesses()
        {
            Configuration.SetAlertedProcesses(list);
            Assert.AreEqual(list.Count, Configuration.GetAlertedProcesses().Count);
            for (int i = 0; i < list.Count; i++) { Assert.AreEqual(list[i], Configuration.GetAlertedProcesses()[i]); }
        }

        [TestMethod]
        public void SetGetAlertedUrls()
        {
            Configuration.SetAlertedUrls(list);
            Assert.AreEqual(list.Count, Configuration.GetAlertedUrls().Count);
            for (int i = 0; i < list.Count; i++) { Assert.AreEqual(list[i], Configuration.GetAlertedUrls()[i]); }
        }

        [TestMethod]
        public void SetGetIgnoredUrls()
        {
            Configuration.SetIgnoredUrls(list);
            Assert.AreEqual(list.Count, Configuration.GetIgnoredUrls().Count);
            for (int i = 0; i < list.Count; i++) { Assert.AreEqual(list[i], Configuration.GetIgnoredUrls()[i]); }
        }

        [TestMethod]
        public void SetGetIgnoredProcesses()
        {
            Configuration.SetIgnoredProcesses(list);
            Assert.AreEqual(list.Count, Configuration.GetIgnoredProcesses().Count);
            for (int i = 0; i < list.Count; i++) { Assert.AreEqual(list[i], Configuration.GetIgnoredProcesses()[i]); }
        }

        [TestMethod]
        public void SetGetFilterEnabled()
        {
            Configuration.SetFilterEnabled(false);
            Assert.AreEqual(false, Configuration.GetFilterEnabled());
            Configuration.SetFilterEnabled(true);
            Assert.AreEqual(true, Configuration.GetFilterEnabled());
        }

        [TestMethod]
        public void SetGetStreamOptions()
        {
            Priority priority = Priority.Widowed;
            List<string> focus = new List<string>() { "test1", "test2", "test3" };
            Configuration.SetStreamOptions(new StreamOptions(priority, focus));
            Assert.AreEqual(priority, Configuration.GetStreamOptions().GetPriority());
            for (int i = 0; i < focus.Count; i++) { Assert.AreEqual(focus[i], Configuration.GetStreamOptions().GetFocus()[i]); }
        }

        [TestMethod]
        public void SetGetFocus()
        {
            Dictionary<string, List<string>> Focus = new Dictionary<string, List<string>>{
                { "test", new List<string>() { "test1", "test2", "test3" } },
                { "test2", new List<string>() { "test10", "test20", "test30","test40" } }};
            Configuration.SetAllFocus(Focus);
            Assert.AreEqual(Focus.Count, Configuration.GetAllFocus().Count);
            Assert.AreEqual(Focus["test"].Count, Configuration.GetAllFocus()["test"].Count);
            Assert.AreEqual(Focus["test2"].Count, Configuration.GetAllFocus()["test2"].Count);
            for (int i = 0; i < Focus["test"].Count; i++) { Assert.AreEqual(Focus["test"][i], Configuration.GetAllFocus()["test"][i]); }
            for (int i = 0; i < Focus["test2"].Count; i++) { Assert.AreEqual(Focus["test2"][i], Configuration.GetAllFocus()["test2"][i]); }
        }
    }
}
