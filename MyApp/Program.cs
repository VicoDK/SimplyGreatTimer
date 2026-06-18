using Avalonia;
using System;
using Avalonia.Diagnostics;

namespace MyApp;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
        => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()   // Avalonia 12 DevTools
#endif
            .WithInterFont()
            .LogToTrace();
}
