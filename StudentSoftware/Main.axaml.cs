using Avalonia.Controls;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Avalonia.Media.Imaging;
using ClassLibrary6;
using ImageMagick;

namespace StudentSoftware;

public partial class Main : Window
{
    #region Variables
    readonly private TrayIcon TrayIconStudent;
    private DataForStudent Student;
    private AskIp WindowAskIp;

    #endregion

    #region Constructor

    public Main()
    {
        InitializeComponent();
        TrayIconStudent = new();
        if (IpForTheWeek.GetIp() == null)
        {
            NewTeacherIP(true);
        }
        else
        {
            InitializeStudent();
        }
        InitializeEvents();
    }

    private void InitializeStudent()
    {
        Student = new(IpForTheWeek.GetIp());
        Student.NewConnexionMessageEvent += (sender,e) => Dispatcher.UIThread.Post(() => lbxInfo.Items.Add(e.Message));
        Student.ChangePropertyEvent += ChangeProperty;
        Student.NewMessageEvent += (sender,e) => lbxInfo.Items.Add(e.Message);
        Student.NewImageEvent += DisplayImage;
    }

    private void InitializeEvents()
    {
        Closing += OnClosing;
        Resized += StudentAppResized;
        TrayIconStudent.Clicked += TrayIconStudentClick;
        btnHelp.Click += HelpReceive;
        btnWebView.Click += WebView2_Click;
        btnChangeIp.Click +=(sender, e) => NewTeacherIP(false);
        btnResetStoredIp.Click += (sender, e) => IpForTheWeek.Reset();
    }

    #endregion

    #region Event Handeling

    /// <summary>
    /// Function that shows the new image.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DisplayImage(object ?sender, NewImageEventArgs e)
    {
        using MemoryStream memStream = new();
        e.image.Write(memStream);
        pbxScreenShot.Source = new Bitmap(memStream);
    }

    /// <summary>
    /// Function that change a property of a control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ChangeProperty(object ?sender, ChangePropertyEventArgs e)
    {
        Control control;
        switch (e.ControlType)
        {
            case ControlType.Image: control = this.FindControl<Image>(e.ControlName);break;
            case ControlType.Window: control = this;break;
            default:return;
        }
        PropertyInfo propInfo = control.GetType().GetProperty(e.PropertyName);
        if (propInfo == null) { return; }
        if (propInfo.CanWrite)
        {
            propInfo.SetValue(control, e.PropertyValue);
        }
    }

    #endregion

    #region teacher ip

    /// <summary>
    /// Function that asks to the student the new teacher ip.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void NewTeacherIP(bool isnew)
    {
        Hide();
        WindowAskIp = new(!isnew);
        WindowAskIp.Activate();
        WindowAskIp.Show();
        WindowAskIp.Closed += (sender, e) =>{
            Show();
            if (isnew)
            {
                InitializeStudent();
            }
            else { Student.IpToTeacher = IpForTheWeek.GetIp(); }
        };
    }

    /// <summary>
    /// Function that verify the interfaces, if they are incorrect it returns a script.
    /// </summary>
    /// <returns> The command to excecute in powershell</returns>
    public string CheckInterfaces()
    {
        NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();
        StringBuilder commandBuilder = new();
        foreach (NetworkInterface current in netInterface)
        {
            bool isCorrect = false;
            IPInterfaceProperties properities = current.GetIPProperties();
            foreach (UnicastIPAddressInformation ip in properities.UnicastAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork))
            {
                if (IsOnSameNetwork(Student.IpToTeacher, ip.Address, ip.IPv4Mask)) { isCorrect = true; }
            }

            commandBuilder.Append("netsh interface ipv4 set interface \"");
            commandBuilder.Append(current.Name);
            if (!isCorrect) { commandBuilder.Append("\" forwarding=disable\r\n"); }
            else { commandBuilder.Append("\" forwarding=enable\r\n"); }
        }
        return commandBuilder.ToString();
    }

    /// <summary>
    /// Function to know if two adresses are on the same network.
    /// </summary>
    /// <param name="ipAddress1">The first ip.</param>
    /// <param name="ipAddress2">The second ip.</param>
    /// <param name="subnetMask">The adress mask.</param>
    /// <returns>If the addresses are on the same network.</returns>
    public static bool IsOnSameNetwork(IPAddress ipAddress1, IPAddress ipAddress2, IPAddress subnetMask)
    {
        byte[] ipBytes1 = ipAddress1.GetAddressBytes();
        byte[] ipBytes2 = ipAddress2.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        for (int i = 0; i < ipBytes1.Length; i++)
        {
            if ((ipBytes1[i] & subnetMaskBytes[i]) != (ipBytes2[i] & subnetMaskBytes[i])) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Function to help the user configure its interfaces.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void HelpReceive(object ?sender, EventArgs e)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\ActiverInterface.ps1");
        using (StreamWriter fichier = new(path))
        {
            fichier.WriteLine(CheckInterfaces());
            fichier.Close();
        }

        MessageBoxManager.GetMessageBoxStandard(
            "Attention",
            "Vos interfaces réseau ne sont pas configurés correctement.\r\n" +
            "1) Lancez une fenêtre windows powershell en administrateur.\r\n" +
            "2) Copiez tout le contenu du fichier " + path + ".\r\n" +
            "3) Collez le tout dans le terminal et executez.\r\n",
            ButtonEnum.YesNo);
    }

    #endregion

    #region interaction

    /// <summary>
    /// Function that communicate to the teacher the closure of the application.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnClosing(object ?sender, WindowClosingEventArgs e)
    {
        TrayIconStudent.Dispose();
        if (Student == null) { return; }
        if (Student.SocketToTeacher == null) { return; }
        Student.SocketToTeacher.Send(Encoding.ASCII.GetBytes("stop"));
        Student.SocketToTeacher.Disconnect(false);
    }

    /// <summary>
    /// Function that displays the trayicon if the application is minimized.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void StudentAppResized(object ?sender, WindowResizedEventArgs e)
    {
        switch (WindowState)
        {
            case WindowState.Minimized:
                TrayIconStudent.IsVisible = true;
                break;
            case WindowState.Normal:
            case WindowState.Maximized:
                TrayIconStudent.IsVisible = false;
                break;
        }
    }

    /// <summary>
    /// Function to show the application when the trayicon is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TrayIconStudentClick(object ?sender, EventArgs e)
    {
        Show();
        TrayIconStudent.IsVisible = false;
        WindowState = WindowState.Normal;
    }

    #endregion

    #region Custom browser

    /// <summary>
    /// Function that creates a new custom browser.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WebView2_Click(object ?sender, EventArgs e)
    {
        /*Form form = new();
        Browser browser = new();
        browser.NewTabEvent += new EventHandler<NewTabEventArgs>(AddWebview2Url);
        form.Controls.Add(browser);
        form.Show();*/
    }

    /// <summary>
    /// Function
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /*private void AddWebview2Url(object sender, NewTabEventArgs e)
    {
        Student.Urls.AddUrl(e.Url, BrowserName.Webview2);
    }*/

    #endregion
}