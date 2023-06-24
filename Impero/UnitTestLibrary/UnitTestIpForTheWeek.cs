using LibraryData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestIpForTheWeekClass
{
    [TestClass]
    public class UnitTestIpForTheWeek
    {
        [TestMethod]
        public void IpForTheWeekConstructor()
        {
            IpForTheWeek Default = new IpForTheWeek();
            IpForTheWeek Instance = new IpForTheWeek("192.168.1.38");
            IpForTheWeek Copy = new IpForTheWeek(Instance);
            Assert.AreEqual(Default.Days["monday"][0], "1.1.1.1");
            Assert.AreEqual(Instance.Days["monday"][0], "192.168.1.38");
            Assert.AreEqual(Copy.Days["monday"][0], "192.168.1.38");
        }

        public void IpForTheWeekSetGet()
        {
            IpForTheWeek Default = new IpForTheWeek();
            Default.SetIp("2.2.2.2");
            Assert.AreEqual(Default.GetIp(),"2.2.2.2");
        }
    }
}
