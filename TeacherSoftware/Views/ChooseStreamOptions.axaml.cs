using System.Collections.Generic;
using Avalonia.Controls;
using ClassLibrary6.Data;

namespace TeacherSoftware.Views;

public partial class ChooseStreamOptions : Window
{
    public ChooseStreamOptions(List<DataForTeacher> students)
    {
        InitializeComponent();
    }
}