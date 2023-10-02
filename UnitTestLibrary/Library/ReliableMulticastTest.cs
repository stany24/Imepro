using LibraryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestLibrary.Library
{
    public class ReliableMulticastTest
    {
        public void CommunicationTest()
        {
            PictureBox pictureBox = new PictureBox();
            ReliableMulticastSender sender = new ReliableMulticastSender(GetUDPMulticastSocket(45678), 0);
            ReliableMulticastReceiver receiver = new ReliableMulticastReceiver(GetUDPMulticastSocket(12345), pictureBox);
            Assert.IsNotNull(pictureBox.Image);
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
    }
}
