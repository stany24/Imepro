using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClassLibrary6.Data;
using ClassLibrary6.History;
using CommunityToolkit.Mvvm.ComponentModel;
using TeacherSoftware.Logic;
using TeacherSoftware.Views;

namespace TeacherSoftware.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public readonly Main MainWindow;
    private readonly List<IndividualStudentWindow> DisplayStudents = new();
    private ConfigurationWindow? _configurationWindow;
    private StreamOptionsWindow? _streamOptionsWindow;
    
    [ObservableProperty] private bool _isEnabled = true;
    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private string _lblIpText = "";
    [ObservableProperty] private string _btnShareContent = "Share";
    [ObservableProperty] private string _btnFilterContent = "Filter";
    [ObservableProperty] private List<Preview> _previews = new();
    [ObservableProperty] private List<string> _allInfos = new();
    
    public ObservableCollection<DataForTeacher> Students { get; set; } = new();

    [ObservableProperty] private int _sliderZoomValue = 50;

    public MainViewModel()
    {
        MainWindow = new Main
        {
            DataContext = this
        };
        MainWindow.Closing+=(_,_) => Closing();
        MainWindow.Show();
        LblIpText = "IP: 192.168.1.1";
        History history = new();
        history.AddUrl(new Url(DateTime.Now, "youtube.com"),BrowserName.Firefox);
        Students.Add(new DataForTeacher(new Data("stan","computer",history,new Dictionary<int, string>(){{367,"rider"}})));
    }

    private void Closing()
    {
        foreach (IndividualStudentWindow displayStudent in DisplayStudents)
        {
            displayStudent.Close();
        }
        _configurationWindow?.Close();
        _streamOptionsWindow?.Close();
    }
    
    public void OpenConfigurationWindow()
    {
        _configurationWindow = new ConfigurationWindow
        {
            DataContext = this
        };
        _configurationWindow.Show();
    }
    
    public void OpenStreamOptionsWindow()
    {
        _streamOptionsWindow = new StreamOptionsWindow
        {
            DataContext = this
        };
        _streamOptionsWindow.Show();
    }
}