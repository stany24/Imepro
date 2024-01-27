using System.Collections.Generic;
using ClassLibrary6.Data;

namespace TeacherSoftware.ViewModels;

public class MainViewModel : ViewModelBase
{
    public List<DataForTeacher> Students { get; set; } = new();
    public bool TrayIconVisible { get; set; } = false;
}