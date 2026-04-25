using System.Diagnostics;
using Avalonia;
using System.Runtime.Versioning;
using System.Text;
using HotAvalonia;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using VRChatContentPublisher.App.Extensions;
using VRChatContentPublisher.Core.Extensions;
using VRChatContentPublisher.Core.Services.App;
using VRChatContentPublisher.Core.Utils;
using VRChatContentPublisher.IpcCore;
using VRChatContentPublisher.IpcCore.Exceptions;
using VRChatContentPublisher.IpcCore.Extensions;
using VRChatContentPublisher.IpcCore.Models;

#if WINDOWS
using VRChatContentPublisher.Platform.Windows.Extensions;
#else
using VRChatContentPublisher.Platform.Noop.Extensions;
#endif

namespace VRChatContentPublisher.App;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static void Main(string[] args)
    {
        var jsonLogPath = Path.Combine(AppStorageService.GetLogsPath(), "log-.json");
        var plainTextLogPath = Path.Combine(AppStorageService.GetLogsPath(), "log-.log");
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "VRChatContentPublisher")
            .Enrich.WithProperty("ApplicationVersion", AppVersionUtils.GetAppVersion())
            .Enrich.WithProperty("ApplicationBuildDate", AppVersionUtils.GetAppBuildDate())
            .Enrich.WithProperty("ApplicationCommitHash", AppVersionUtils.GetAppCommitHash())
            .WriteTo.Console(applyThemeToRedirectedOutput: true, theme: AnsiConsoleTheme.Code)
            .WriteTo.Async(writer =>
                writer.File(new CompactJsonFormatter(), jsonLogPath,
                    rollingInterval: RollingInterval.Day))
            .WriteTo.Async(writer =>
                writer.File(plainTextLogPath, rollingInterval: RollingInterval.Day))
            .WriteTo.Debug()
            .CreateLogger();

        Log.Information(
            "VRChat Content Publisher v{AppVersion} built on {AppBuildDate} (commit {AppCommitHash}) starting up...",
            AppVersionUtils.GetAppVersion(),
            AppVersionUtils.GetAppBuildDate(),
            AppVersionUtils.GetAppCommitHash()
        );


        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            if (eventArgs.ExceptionObject is not Exception ex)
            {
                Log.Write(
                    eventArgs.IsTerminating ? Serilog.Events.LogEventLevel.Fatal : Serilog.Events.LogEventLevel.Error,
                    "An unhandled exception of type {ExceptionType} was thrown: {ExceptionObject}, IsTerminating: {IsTerminating}",
                    eventArgs.ExceptionObject.GetType(),
                    eventArgs.ExceptionObject,
                    eventArgs.IsTerminating
                );

#if RELEASE
                TryLaunchCrashHandler(eventArgs.ExceptionObject);
#endif
                return;
            }

            Log.Write(
                eventArgs.IsTerminating ? Serilog.Events.LogEventLevel.Fatal : Serilog.Events.LogEventLevel.Error,
                ex, "An unhandled exception was thrown, IsTerminating: {IsTerminating}",
                eventArgs.IsTerminating
            );

#if RELEASE
            TryLaunchCrashHandler(ex);
#endif
        };

        try
        {
            using var appMutex = new AppMutex();

            try
            {
                appMutex.OwnMutex();
            }
            catch (AbandonedMutexException ex)
            {
                Log.Warning(ex,
                    "The previous instance of the application did not release the mutex properly. " +
                    "Continuing to run this instance.");
            }

            var builder = new HostApplicationBuilder();

            builder.Services.AddSerilog();

            builder.UseAppCore();
            builder.Services.AddAppServices();
            builder.Services.AddIpcCore();

#if WINDOWS
            builder.Services.AddWindowsPlatformServices();
#else
            builder.Services.AddNoopPlatformServices();
#endif

            builder.Services.AddAvaloniaApplication<App>(appBuilder => appBuilder
                .UseHotReload()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace());

            using var host = builder.Build();

            host.RunAvaloniaWaitForShutdown(args);
        }
        catch (MutexOwnedByAnotherInstanceException)
        {
            Log.Information("Another instance is already running. Exiting this instance.");
            Environment.ExitCode = -1;

            try
            {
                var ipcClient = new IpcClient();
                ipcClient.SendIpcCommand(IpcCommand.ActivateWindow);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send IPC command to the existing instance.");
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Oops, the application has crashed!");
            Environment.ExitCode = -1;

#if RELEASE
            TryLaunchCrashHandler(ex);
#endif
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void TryLaunchCrashHandler(object ex)
    {
        try
        {
            LaunchCrashHandler(ex);
        }
        catch (Exception launchEx)
        {
            Log.Error(launchEx, "Failed to launch the crash handler.");
        }
    }

    private static void LaunchCrashHandler(object ex)
    {
        var crashHandlerExecutable =
            OperatingSystem.IsWindows()
                ? "VRChatContentPublisher.CrashHandler.App.exe"
                : "VRChatContentPublisher.CrashHandler.App";
        var crashHandlerPath = Path.Combine(AppContext.BaseDirectory, "CrashHandler",
            crashHandlerExecutable);
        var fallbackAppPath = Path.Combine(AppContext.BaseDirectory,
            "VRChatContentPublisher.App" + (OperatingSystem.IsWindows() ? ".exe" : "")
        );

        if (!File.Exists(crashHandlerPath))
        {
            throw new FileNotFoundException($"Crash handler executable not found at path: {crashHandlerPath}");
        }

        var errorText = ex.ToString() ?? $"Unhandled Exception with non-exception type: {ex.GetType()}\nToString: {ex}";

        using var process = Process.Start(crashHandlerPath, [
            "--exception",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(errorText)),
            "--logsFolderPath",
            AppStorageService.GetLogsPath(),
            "--applicationPath",
            Environment.ProcessPath ?? fallbackAppPath
        ]);
    }
}