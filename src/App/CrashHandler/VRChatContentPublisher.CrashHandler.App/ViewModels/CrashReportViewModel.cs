using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;

namespace VRChatContentPublisher.CrashHandler.App.ViewModels;

public sealed partial class CrashReportViewModel : ViewModelBase
{
    private const string SampleException = """
                                           System.InvalidOperationException: Test
                                              at UserQuery.<>c.<Main>b__9_0() in C:\Users\Misak\AppData\Local\Temp\LINQPad9\_aixervlk\oriycp\LINQPadQuery:line 5
                                              at System.Threading.Tasks.Task`1.InnerInvoke()
                                              at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
                                           --- End of stack trace from previous location ---
                                              at System.Threading.ExecutionContext.RunFromThreadPoolDispatchLoop(Thread threadPoolThread, ExecutionContext executionContext, ContextCallback callback, Object state)
                                              at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
                                           --- End of stack trace from previous location ---
                                              at UserQuery.Main() in C:\Users\Misak\AppData\Local\Temp\LINQPad9\_aixervlk\oriycp\LINQPadQuery:line 4
                                           """;

    // All information (include the logs folder path) are provide by main app.
    // Following value are for previewer.
    public string Exception { get; set; } = SampleException;

    public string LogsFolderPath { get; set; } = AppPathUtils.GetLogsPath();

    public string ApplicationPath = AppPathUtils.GetMainAppPath();

    [RelayCommand]
    private void CloseAndRestart()
    {
        if (File.Exists(ApplicationPath))
        {
            using var process = Process.Start(ApplicationPath);
        }

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
            return;

        desktop.Shutdown();
    }
}