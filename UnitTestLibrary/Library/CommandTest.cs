using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryData;
using System.Collections.Generic;

namespace UnitTestLibrary.Library
{
    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public void GetSetCommand()
        {
            Command cmd1 = new Command(CommandType.DemandData);
            Assert.AreEqual(CommandType.DemandData, cmd1.Type);
            Assert.IsNotNull(cmd1.Args);

            List<string> args = new List<string>() {"test1","test2" };
            Command cmd2 = new Command(CommandType.ApplyMulticastSettings, args);
            Assert.AreEqual(CommandType.ApplyMulticastSettings, cmd2.Type);
            Assert.AreEqual(args,cmd2.Args);
        }
    }
}
