using ApplicationCliente;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestLibrary.Student
{
    [TestClass]
    public class IpForTheWeekTest
    {
        /// <summary>
        /// Test fails because settings.settings cannot be loaded by another project.
        /// </summary>
        [TestMethod]
        public void UnitTestGetSet()
        {
            string NotIp = "gdijgas";
            string Ip = "192.168.1.38";
            IpForTheWeek.SetIp(Ip);
            Assert.AreEqual(IpForTheWeek.GetIp().ToString(), Ip);
            IpForTheWeek.SetIp(NotIp);
            Assert.AreEqual(IpForTheWeek.GetIp().ToString(), NotIp);
        }
    }
}
