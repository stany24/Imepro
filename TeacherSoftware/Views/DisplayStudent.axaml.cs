using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ClassLibrary6.Data;

namespace TeacherSoftware.Views;

public partial class DisplayStudent : Window
{
    private int _id;
    public DisplayStudent(int id)
    {
        _id = id;
        InitializeComponent();
    }

    public int GetStudentId()
    {
        return _id;
    }

    public void UpdateDisplay(DataForTeacher student)
    {
        
    }
}