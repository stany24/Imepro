using LibraryData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestLibrary.Library
{
    [TestClass]
    public class StreamOptionsTest
    {
        [TestMethod]
        public void StreamOptionsConstructor()
        {
            Priority priority = Priority.Topmost;
            List<string> strings = new List<string> { "test1", "test2", "test3" };
            StreamOptions options = new StreamOptions(priority, strings);
            Assert.AreEqual(options.GetPriority(), priority);
            Assert.AreEqual(options.GetFocus(), strings);
        }
    }
}
