﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryData;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace ApplicationTeacher
{
    public partial class TeacherApp : Form
    {
        readonly List<DataForTeacher> AllClients = new();
        readonly int DurationBetweenDemand = 15;
        readonly int DefaultTimeout = 2000;
        public TeacherApp()
        {
            InitializeComponent();
            Task.Run(LogClients);
            Task.Run(AskingData);
        }

        public void LogClients()
        {
            // Establish the local endpointfor the socket.
            // Dns.GetHostName returns the name of the host running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = null;
            foreach (IPAddress address in ipHost.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork) { ipAddr = address; break; }
            }
            IPEndPoint localEndPoint = new(ipAddr, 11111);
            // Creation TCP/IP Socket using Socket Class Constructor
            Socket listener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Using Bind() method we associate a network address to the Server Socket
            // All client that will connect to this Server Socket must know this network Address
            listener.Bind(localEndPoint);
            // Using Listen() method we create the Client list that will want to connect to Server
            listener.Listen(-1);
            while (true)
            {
                lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("Waiting connection..."); }));
                try
                {
                    // Suspend while waiting for incoming connection Using Accept() method the server will accept connection of client
                    Socket clientSocket = listener.Accept();
                    lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add("New connexion to " + clientSocket.RemoteEndPoint); }));
                    AllClients.Add(new DataForTeacher(clientSocket));
                    /*lbxClients.Invoke(new MethodInvoker(delegate {
                        lbxClients.DataSource = null;
                        lbxClients.DataSource = AllClients;
                    }));*/
                }
                catch (Exception e) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(e.ToString()); })); }
            }
        }

        private void AskingData()
        {
            while (true)
            {
                if (AllClients.Count != 0)
                {
                    DateTime StartUpdate = DateTime.Now;
                    DateTime NextUpdate = DateTime.Now.AddSeconds(DurationBetweenDemand);
                    List<DataForTeacher> ClientToRemove = new();
                    for (int i = 0; i < AllClients.Count; i++)
                    {
                        Socket socket = AllClients[i].SocketToStudent;
                        socket.ReceiveTimeout = DefaultTimeout;
                        socket.SendTimeout = DefaultTimeout;
                        try
                        {
                            //demande les données
                            socket.Send(Encoding.ASCII.GetBytes("data"));
                            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("asked data to client " + AllClients[i].UserName); }));
                            Task.Run(() => AllClients[i] = ReceiveData(AllClients[i])).Wait();
                            //demande le screenshot
                            socket.Send(Encoding.ASCII.GetBytes("image"));
                            lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("asked image to client " + AllClients[i].UserName); }));
                            Task.Run(() => ReceiveImage(AllClients[i])).Wait();
                            lbxClients.Invoke(new MethodInvoker(delegate { lbxClients.Items.Add(AllClients[i].ToString()); }));
                            pbxScreenShot.Invoke(new MethodInvoker(delegate { pbxScreenShot.Image = AllClients[i].ScreenShot; }));
                        }
                        catch
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                            ClientToRemove.Add(AllClients[i]);
                        }
                    }
                    foreach (DataForTeacher client in ClientToRemove)
                    {
                        AllClients.Remove(client);
                        /*lbxClients.Invoke(new MethodInvoker(delegate {
                            lbxClients.DataSource = null;
                            lbxClients.DataSource = AllClients;
                        }));*/
                        lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("Le client " + client.UserName + "à été retiré"); }));
                    }
                    //foreach (DataForTeacher client in AllClients) { client.UpdateAffichage(); }
                    DateTime FinishedUpdate = DateTime.Now;
                    TimeSpan UpdateDuration = FinishedUpdate - StartUpdate;
                    TimeSpan CycleDuration = NextUpdate - StartUpdate;
                    if (CycleDuration > UpdateDuration)
                    {
                        lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("Attente du prochain cycle dans " + (CycleDuration - UpdateDuration) + " secondes"); }));
                        Thread.Sleep(CycleDuration - UpdateDuration);
                    }
                }
                else { Thread.Sleep(100); }
            }
        }

        private DataForTeacher ReceiveData(DataForTeacher student)
        {
            try
            {
                Socket socket = student.SocketToStudent;
                //AffichageEleve affichage = student.affichage;
                byte[] dataBuffer = new byte[1024];
                socket.ReceiveTimeout = DefaultTimeout;
                int nbData = socket.Receive(dataBuffer);
                Array.Resize(ref dataBuffer, nbData);
                student = new DataForTeacher(JsonSerializer.Deserialize<Data>(Encoding.Default.GetString(dataBuffer)));
                student.SocketToStudent = socket;
                //student.affichage = affichage;
                /*lbxClients.Invoke(new MethodInvoker(delegate {
                    lbxClients.DataSource = null;
                    lbxClients.DataSource = AllClients;
                }));*/
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("data recue de " + student.UserName); }));
                return student;
            }
            catch
            {
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(student.UserName + "n'a pas envoyé de donnée"); }));
                return student;
            }
        }

        private void ReceiveImage(DataForTeacher student)
        {
            try
            {
                Socket socket = student.SocketToStudent;
                byte[] imageBuffer = new byte[10485760];
                socket.ReceiveTimeout = DefaultTimeout;
                int nbData = socket.Receive(imageBuffer, 0, imageBuffer.Length, SocketFlags.None);
                Array.Resize(ref imageBuffer, nbData);
                student.ScreenShot = new Bitmap(new MemoryStream(imageBuffer));
                /*lbxClients.Invoke(new MethodInvoker(delegate {
                    lbxClients.DataSource = null;
                    lbxClients.DataSource = AllClients;
                }));*/
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add("image recue de " + student.UserName); }));
            }
            catch { lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(student.UserName + "n'a pas envoyé d'image"); })); }

        }
    }
}
