using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ClassLibrary6.Data;
using ClassLibrary6.ReliableMulticast;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using StudentSoftware.Logic;
using StudentSoftware.ViewModels;

namespace StudentSoftware.Views;

public partial class Main : Window
{
    #region Variables
    private readonly TrayIcon _trayIconStudent;
    private DataForStudent _student;
    private AskIp _windowAskIp;

    #endregion

    #region Constructor

    public Main(MainViewModel model)
    {
        DataContext = model;
        InitializeComponent();
        _trayIconStudent = new TrayIcon();
        if (IpForTheWeek.GetIp() == null)
        {
            NewTeacherIp(true);
        }
        else
        {
            InitializeStudent();
        }
        InitializeEvents();
    }

    private void InitializeStudent()
    {
        _student = new DataForStudent(IpForTheWeek.GetIp());
        _student.NewConnexionMessageEvent += (sender,e) => Dispatcher.UIThread.Post(() => LbxInfo.Items.Add(e.Message));
        _student.ChangePropertyEvent += ChangeProperty;
        _student.NewMessageEvent += (sender,e) => LbxInfo.Items.Add(e.Message);
        _student.NewImageEvent += DisplayImage;
    }

    private void InitializeEvents()
    {
        Closing += OnClosing;
        Resized += StudentAppResized;
        _trayIconStudent.Clicked += TrayIconStudentClick;
        BtnHelp.Click += HelpReceive;
        BtnWebView.Click += WebView2_Click;
        BtnChangeIp.Click +=(_, _) => NewTeacherIp(false);
        BtnResetStoredIp.Click += (_, _) => IpForTheWeek.Reset();
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
        e.Image.Write(memStream);
        PbxScreenShot.Source = new Bitmap(memStream);
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
    /// <param name="isNew"></param>
    private void NewTeacherIp(bool isNew)
    {
        Hide();
        _windowAskIp = new AskIp(!isNew);
        _windowAskIp.Activate();
        _windowAskIp.Show();
        _windowAskIp.Closed += (_, _) =>{
            Show();
            if (isNew)
            {
                InitializeStudent();
            }
            else { _student.IpToTeacher = IpForTheWeek.GetIp(); }
        };
    }

    /// <summary>
    /// Function that verify the interfaces, if they are incorrect it returns a script.
    /// </summary>
    /// <returns> The command to execute in powershell</returns>
    private string CheckInterfaces()
    {
        NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();
        StringBuilder commandBuilder = new();
        foreach (NetworkInterface current in netInterface)
        {
            bool isCorrect = false;
            IPInterfaceProperties properties = current.GetIPProperties();
            foreach (UnicastIPAddressInformation ip in properties.UnicastAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork))
            {
                if (IsOnSameNetwork(_student.IpToTeacher, ip.Address, ip.IPv4Mask)) { isCorrect = true; }
            }

            commandBuilder.Append("netsh interface ipv4 set interface \"");
            commandBuilder.Append(current.Name);
            commandBuilder.Append(!isCorrect ? "\" forwarding=disable\r\n" : "\" forwarding=enable\r\n");
        }
        return commandBuilder.ToString();
    }

    /// <summary>
    /// Function to know if two addresses are on the same network.
    /// </summary>
    /// <param name="ipAddress1">The first ip.</param>
    /// <param name="ipAddress2">The second ip.</param>
    /// <param name="subnetMask">The address mask.</param>
    /// <returns>If the addresses are on the same network.</returns>
    private static bool IsOnSameNetwork(IPAddress ipAddress1, IPAddress ipAddress2, IPAddress subnetMask)
    {
        byte[] ipBytes1 = ipAddress1.GetAddressBytes();
        byte[] ipBytes2 = ipAddress2.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        return !ipBytes1.Where((t, i) => (t & subnetMaskBytes[i]) != (ipBytes2[i] & subnetMaskBytes[i])).Any();
    }

    /// <summary>
    /// Function to help the user configure its interfaces.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HelpReceive(object ?sender, EventArgs e)
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads\\ActiverInterface.ps1");
        using (StreamWriter file = new(path))
        {
            file.WriteLine(CheckInterfaces());
            file.Close();
        }

        MessageBoxManager.GetMessageBoxStandard(
            "Attention",
            "Vos interfaces r�seau ne sont pas configur�s correctement.\r\n" +
            "1) Lancez une fen�tre windows powershell en administrateur.\r\n" +
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
    private void OnClosing(object ?sender, WindowClosingEventArgs e)
    {
        _trayIconStudent.Dispose();
        if (_student == null) { return; }
        if (_student.SocketToTeacher == null) { return; }
        _student.SocketToTeacher.Send(Encoding.ASCII.GetBytes("stop"));
        _student.SocketToTeacher.Disconnect(false);
    }

    /// <summary>
    /// Function that displays the tray-icon if the application is minimized.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StudentAppResized(object ?sender, WindowResizedEventArgs e)
    {
        switch (WindowState)
        {
            case WindowState.Minimized:
                _trayIconStudent.IsVisible = true;
                break;
            case WindowState.Normal:
            case WindowState.Maximized:
                _trayIconStudent.IsVisible = false;
                break;
        }
    }

    /// <summary>
    /// Function to show the application when the tray-icon is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrayIconStudentClick(object ?sender, EventArgs e)
    {
        Show();
        _trayIconStudent.IsVisible = false;
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