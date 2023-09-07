using LibraryData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace UnitTestLibrary.Library
{
    [TestClass]
    public class DataTest
    {
        [TestMethod]
        public void Constructors()
        {
            Data data1 = new Data();
            Assert.IsNotNull(data1.Urls);
            Assert.IsNotNull(data1.Processes);
            const string username = "username";
            const string computername = "computername";
            History historique = new History();
            historique.AddUrl(new Url(DateTime.Now, "test"), BrowserName.Firefox);
            Dictionary<int, string> processes = new Dictionary<int, string> { { 1, "firefox" } };
            Data data2 = new Data(username, computername, historique, processes);
            Assert.AreEqual(username, data2.UserName);
            Assert.AreEqual(computername, data2.ComputerName);
            Assert.AreEqual(historique, data2.Urls);
            Assert.AreEqual(processes, data2.Processes);
        }
    }

    [TestClass]
    public class DataForTeacherTest
    {
        [TestMethod]
        public void Constructors()
        {
            const int id = 3;
            Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            DataForTeacher dataForTeacher1 = new DataForTeacher(socket, id);
            Assert.AreEqual(id, dataForTeacher1.ID);
            Assert.AreEqual(socket, dataForTeacher1.SocketToStudent);
            const string username = "username";
            const string computername = "computername";
            History historique = new History();
            historique.AddUrl(new Url(DateTime.Now, "test"), BrowserName.Firefox);
            Dictionary<int, string> processes = new Dictionary<int, string> { { 1, "firefox" } };
            Data data = new Data(username, computername, historique, processes);
            DataForTeacher dataForTeacher2 = new DataForTeacher(data);
            Assert.AreEqual(data.UserName, dataForTeacher2.UserName);
            Assert.AreEqual(data.ComputerName, dataForTeacher2.ComputerName);
            Assert.AreEqual(data.Urls, dataForTeacher2.Urls);
            Assert.AreEqual(data.Processes, dataForTeacher2.Processes);
        }
    }

    [TestClass]
    public class DataForStudentTest
    {
    }
}
