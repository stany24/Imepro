using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using TeacherSoftware.ViewModels;
using TeacherSoftware.Views;

namespace TeacherSoftware;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainViewModel model = new();
            desktop.MainWindow = model.MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}