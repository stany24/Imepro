using Avalonia.Controls;
using ClassLibrary6.Data;

namespace TeacherSoftware.Views;

public partial class IndividualStudentWindow : Window
{
    private int _id;
    public IndividualStudentWindow(int id)
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