using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Library;

namespace TeacherApp;

public partial class MainWindow : Window
{
    readonly private PreviewDisplayer Displayer;
    readonly private List<DataForTeacher> AllStudents = new();
    readonly private List<DisplayStudent> AllStudentsDisplay = new();

    private List<DataForTeacher> StudentToShareScreen = new();
    private Task ScreenSharer;
    private bool isSharing = false;
    private bool isAsking = false;
    private int NextId = 0;
    private IPAddress ipAddr = null;
    private ReliableMulticastSender MulticastSender;
    
    public MainWindow()
    {
        InitializeComponent();
        FindIp();
        Task.Run(StartTasks);
    }

    private void StartTasks()
    {
        Task.Run(AskingData);
        Task.Run(LogClients);
    }

    private void FindIp()
    {
        // Establish the local endpointfor the socket.
        // Dns.GetHostName returns the name of the host running the application.
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> PossiblesIp = new();
        foreach (IPAddress address in ipHost.AddressList)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork) { PossiblesIp.Add(address); }
        }
        switch (PossiblesIp.Count)
        {
            case 0:
                MessageBox.Show("Aucune addresse ip conforme n'a étée trouvée.\r\n" +
                                "Vérifiez vos connexion aux réseaux.\r\n" +
                                "L'application va ce fermer.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
                break;
            case 1: ipAddr = PossiblesIp[0]; break;
            default:
                AskToChoseIp prompt = new(PossiblesIp);
                if (prompt.ShowDialog(this) == DialogResult.OK) { ipAddr = prompt.GetChoosenIp(); }
                else { ipAddr = PossiblesIp[0]; }
                prompt.Dispose();
                break;
        }
        lblIP.Text = "IP: " + ipAddr.ToString();
    }
    
    private void AskingData()
    {
        while (true)
        {
            if (AllStudents.Count != 0)
            {
                while (isAsking) { Thread.Sleep(10); }
                isAsking = true;
                DateTime StartUpdate = DateTime.Now;
                DateTime NextUpdate = DateTime.Now.AddSeconds(15);
                List<DataForTeacher> ClientToRemove = new();
                UpdateEleves(ClientToRemove);
                foreach (DataForTeacher client in ClientToRemove)
                {
                    RemoveStudent(client);
                }
                UpdateAllIndividualDisplay();
                DateTime FinishedUpdate = DateTime.Now;
                TimeSpan UpdateDuration = FinishedUpdate - StartUpdate;
                TimeSpan CycleDuration = NextUpdate - StartUpdate;
                if (CycleDuration > UpdateDuration)
                {
                    isAsking = false;
                    lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Attente du prochain cycle dans " + (CycleDuration - UpdateDuration) + " secondes"); }));
                    Thread.Sleep(CycleDuration - UpdateDuration);
                }
            }
            else { Thread.Sleep(100); }
        }
    }

    private void LogClients()
    {
        IPEndPoint localEndPoint = new(ipAddr, 11111);
        Socket listener = new(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(localEndPoint);
        listener.Listen(-1);
        while (true)
        {
            try
            {
                Socket clientSocket = listener.Accept();
                
                lbxRequetes.Invoke(new MethodInvoker(delegate { lbxRequetes.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " Nouvelle connexion de: " + clientSocket.RemoteEndPoint); }));
                AllStudents.Add(new DataForTeacher(clientSocket, NextId));
                Task.Run(() => SendAutorisedUrl(clientSocket));
                NextId++;
            }
            catch (Exception e) { lbxConnexion.Invoke(new MethodInvoker(delegate { lbxConnexion.Items.Add(e.ToString()); })); }
        }
    }
}