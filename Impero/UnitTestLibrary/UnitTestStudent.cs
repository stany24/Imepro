using ApplicationCliente;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestStudent
{
    [TestClass]
    public class UnitTestIpForTheWeek
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
            Assert.AreEqual(IpForTheWeek.GetIp().ToString(), Ip);
        }
    }
}
