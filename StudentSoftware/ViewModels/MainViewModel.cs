using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Avalonia.Media.Imaging;
using ClassLibrary6.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using ImageMagick;
using StudentSoftware.Logic;
using StudentSoftware.Views;

namespace StudentSoftware.ViewModels;

public class MainViewModel: ObservableObject
{
    public DataForStudent Student { get; set; } = new(IPAddress.Parse("192.168.56.1"));
    
    private ObservableCollection<string> _messages = new();
    public ObservableCollection<string> Messages { 
        get => _messages;
        set => SetProperty(ref _messages, value);
    }
    
    private ObservableCollection<string> _infos = new();
    public ObservableCollection<string> Infos { 
        get => _infos;
        set => SetProperty(ref _infos, value);
    }
    
    public Main MainWindow { get; set; }
    public Bitmap ScreenShot { get; set; }
    private AskIp? _askIpWindow;
    
    public MainViewModel()
    {
        MainWindow = new Main(this); 
        ScreenShot = TakeScreenShot();
        Student.NewConnexionMessageEvent += (_,e) => Messages.Add(e.Message);
    }
    
    public void NewTeacherIP()
    {
        if(_askIpWindow != null) { return; }
        _askIpWindow = new AskIp(false);
        _askIpWindow.ShowDialog(_askIpWindow);
    }

    public void ShowWebview()
    {
        
    }

    public void ShowHelp()
    {
        
    }
    
    private Bitmap TakeScreenShot()
    {
        using MemoryStream memStream = new();
        MagickImageCollection images = new();
        images.Read("SCREENSHOT", MagickFormat.Screenshot);
        MagickImage image = new(images.AppendHorizontally());
        using MemoryStream stream = new(image.ToByteArray(MagickFormat.Jpg));
        return new Bitmap(stream);
    }
}