using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Library;

namespace TeacherApp;

public partial class StudentWindow : Window
{
    private DataForTeacher student;
    public StudentWindow()
    {
        InitializeComponent();
    }

    public int GetStudentId()
    {
        return student.Id;
    }

    public void UpdateAffichage(DataForTeacher student)
    {
        
    }
}