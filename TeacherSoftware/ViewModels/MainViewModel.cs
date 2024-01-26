using System;
using System.Collections.Generic;
using ClassLibrary6.Data;
using ClassLibrary6.History;


namespace TeacherSoftware.ViewModels;

public class MainViewModel : ViewModelBase
{
    public List<DataForTeacher> Students { get; set; } = new();
    public bool TrayIconVisible { get; set; } = false;

    public MainViewModel()
    {
        History history = new();
        history.AddUrl(new(DateTime.Now, "testf"),BrowserName.Firefox);
        history.AddUrl(new(DateTime.Now, "testc"),BrowserName.Chrome);
        Students.Add(new DataForTeacher(new Data("testname",
            "computername",
            history,
            new Dictionary<int, string>{{1,"test1"},{2,"test2"}})));
    }
}