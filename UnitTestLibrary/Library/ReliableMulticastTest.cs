using LibraryData;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestLibrary.Library
{
    [TestClass]
    public class ReliableMulticastTest
    {
        [TestMethod]
        public void CommunicationTest()
        {
            PictureBox pictureBox = new PictureBox();
            ReliableMulticastSender sender = new ReliableMulticastSender(GetUDPMulticastSocket(45678), 0);
            ReliableMulticastReceiver receiver = new ReliableMulticastReceiver(GetUDPMulticastSocket(12345), pictureBox);

        }

        private Socket GetUDPMulticastSocket(int port)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress ip = IPAddress.Parse("232.1.2.3");
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, IPAddress.Parse("232.1.2.3").GetAddressBytes());
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 10);
            IPEndPoint ipep = new IPEndPoint(ip, port);
            s.Connect(ipep);
            return s;
        }

        [TestMethod]
        public void CustomStringTest()
        {
            ReliableMulticastSender sender = new ReliableMulticastSender(GetUDPMulticastSocket(45678), 0);
            byte[] image = sender.TakeScreenshot();
            //first message
            byte[] data = new byte[64000];
            Array.Copy(image, 0, data, 0, 64000);
            ReliableMulticastMessage message = new ReliableMulticastMessage(data,1,1,2);
            string str = message.ToCustomString();
            ReliableMulticastMessage message2 = new ReliableMulticastMessage(str);
            Assert.AreEqual(message.ImageNumber, message2.ImageNumber);
            Assert.AreEqual(message.TotalPartNumber, message2.TotalPartNumber);
            Assert.AreEqual(message.PartNumber, message2.PartNumber);
            Assert.AreEqual(message.Data.Count(), message2.Data.Count());
            Assert.IsTrue(message.Data.SequenceEqual(message2.Data));
            Assert.IsTrue(ByteArrayCompare(message.Data,message2.Data));
            //second message
            byte[] data2 = new byte[image.Length - 64000];
            Array.Copy(image,64000,data2,0, image.Length-64000);
            ReliableMulticastMessage message3 = new ReliableMulticastMessage(data2,1, 2, 2);
            string str2 = message3.ToCustomString();
            ReliableMulticastMessage message4 = new ReliableMulticastMessage(str2);
            Assert.AreEqual(message3.ImageNumber, message4.ImageNumber);
            Assert.AreEqual(message3.TotalPartNumber, message4.TotalPartNumber);
            Assert.AreEqual(message3.PartNumber, message4.PartNumber);
            Assert.AreEqual(message3.Data.Count(), message4.Data.Count());
            Assert.IsTrue(message3.Data.SequenceEqual(message4.Data));
            Assert.IsTrue(ByteArrayCompare(message3.Data, message4.Data));
        }

        static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }
    }
}
