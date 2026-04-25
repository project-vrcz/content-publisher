using System.Diagnostics;
using System.Text;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using VRChatContentPublisher.CrashHandler.App.ViewModels;
using VRChatContentPublisher.CrashHandler.App.Views;

namespace VRChatContentPublisher.CrashHandler.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ViewLocator.Register<CrashReportViewModel, CrashReportView>();

        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            throw new InvalidOperationException("This app only support classic desktop application lifetime.");
        }

        var vm = GetViewModel();
        if (vm is null)
        {
            Environment.Exit(0);
            return;
        }

        desktop.MainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel
            {
                CurrentView = vm
            }
        };

        base.OnFrameworkInitializationCompleted();
    }

    private static ViewModelBase? GetViewModel()
    {
        var args = GetCommandLineArguments(Environment.GetCommandLineArgs());
        if (!args.TryGetValue("exception", out var exceptionRawString))
        {
            LaunchMainApp();
            return null;
        }

        var exceptionString = Encoding.UTF8.GetString(Convert.FromBase64String(exceptionRawString));;
        var crashReportViewModel = new CrashReportViewModel
        {
            Exception = exceptionString
        };

        crashReportViewModel.LogsFolderPath =
            args.GetValueOrDefault("logsFolderPath", crashReportViewModel.LogsFolderPath);
        crashReportViewModel.ApplicationPath =
            args.GetValueOrDefault("applicationPath", crashReportViewModel.ApplicationPath);

        return crashReportViewModel;
    }

    private static void LaunchMainApp()
    {
        var appPath = AppPathUtils.GetMainAppPath();
        if (!File.Exists(appPath))
            return;

        using var process = Process.Start(appPath);
    }

    private static Dictionary<string, string> GetCommandLineArguments(string[] args)
    {
        var dict = new Dictionary<string, string>();

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--"))
            {
                var key = args[i].Substring(2);
                var value = (i + 1 < args.Length && !args[i + 1].StartsWith("--")) ? args[i + 1] : "true";
                dict[key] = value;
            }
        }

        return dict;
    }
}