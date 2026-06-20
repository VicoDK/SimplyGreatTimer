using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MyApp.ViewModels;
using MyApp.Views;

namespace MyApp;

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
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        /*AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            File.WriteAllText(
                Path.Combine(AppContext.BaseDirectory, "crash.txt"),
                e.ExceptionObject.ToString());
        };*/

        base.OnFrameworkInitializationCompleted();
    }
}