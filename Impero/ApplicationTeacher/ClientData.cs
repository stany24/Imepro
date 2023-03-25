using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace ApplicationCliente
{
    public class ClientData
    {
        public String UserName = "";
        public String ComputerName = "";
        public List<String> Urls = new();
        public Dictionary<int, String> Processes = new();
        public Bitmap ScreenShot;

        public ClientData(string userName, string computerName, List<string> urls, Dictionary<int, string> processes, Bitmap screenShot)
        {
            UserName = userName;
            ComputerName = computerName;
            Urls = urls;
            Processes = processes;
            ScreenShot = screenShot;
        }

        public ClientData() { }
    }

    public class ClientDataForTeacher : ClientData
    {
        public Socket SocketToClient;

        public ClientDataForTeacher(Socket socket)
        {
            SocketToClient = socket;
        }
    }

    public class ClientDataForClient : ClientData
    {
        public Socket SocketSocketToTeacher;
        readonly public List<string> browsersList = new() { "chrome", "firefox", "iexplore", "safari", "opera", "msedge" };

        public void GetNames()
        {
            ComputerName = System.Environment.MachineName;
            UserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        public void GetUrl()
        {
            [DllImport("user32.dll")]
             static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            foreach (string singleBrowser in browsersList)
            {
                Process[] process = Process.GetProcessesByName(singleBrowser);
                if (process.Length > 0)
                {
                    foreach (Process instance in process)
                    {
                        IntPtr hWnd = instance.MainWindowHandle;

                        StringBuilder text = new(GetWindowTextLength(hWnd) + 1);
                        _ = GetWindowText(hWnd, text, text.Capacity);
                        if (text.ToString() != "") { Urls.Add(singleBrowser + " : " + text.ToString()); }
                    }
                }
            }

        }
    }
}
