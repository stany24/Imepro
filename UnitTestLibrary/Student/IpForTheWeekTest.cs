﻿using ApplicationCliente;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestLibrary.Student
{
    [TestClass]
    public class IpForTheWeekTest
    {
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
