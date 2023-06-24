using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryData;
using System.Collections.Generic;

namespace UnitTestStreamOptionClass
{
    [TestClass]
    public class UnitTestStreamOptions
    {
        [TestMethod]
        public void StreamOptionsConstructor()
        {
            Priority priority = Priority.Topmost;
            Focus focus = Focus.Word;
            List<string> strings = new List<string> { "test1", "test2", "test3" };   
            StreamOptions options = new StreamOptions(priority, focus,strings);
            Assert.AreEqual(options.priority, priority);
            Assert.AreEqual(options.focus, focus);
            Assert.AreEqual(options.AutorisedOpenedProcess, strings);
        }
    }
}
