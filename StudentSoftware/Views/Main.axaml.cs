using Avalonia.Controls;
using StudentSoftware.ViewModels;

namespace StudentSoftware.Views;

public partial class Main : Window
{
    public Main(MainViewModel model)
    {
        DataContext = model;
        InitializeComponent();
    }
}