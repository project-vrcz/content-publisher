using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Pages;
using VRChatContentPublisher.App.Pages.GettingStarted;
using VRChatContentPublisher.App.Pages.HomeTab;
using VRChatContentPublisher.App.Pages.Settings;
using VRChatContentPublisher.App.ViewModels;
using VRChatContentPublisher.App.ViewModels.Data;
using VRChatContentPublisher.App.ViewModels.Data.Connect;
using VRChatContentPublisher.App.ViewModels.Data.PublishTasks;
using VRChatContentPublisher.App.ViewModels.Dialogs;
using VRChatContentPublisher.App.ViewModels.InAppNotifications;
using VRChatContentPublisher.App.ViewModels.Pages;
using VRChatContentPublisher.App.ViewModels.Pages.GettingStarted;
using VRChatContentPublisher.App.ViewModels.Pages.HomeTab;
using VRChatContentPublisher.App.ViewModels.Pages.Settings;
using VRChatContentPublisher.App.ViewModels.Settings;
using VRChatContentPublisher.App.Views;
using VRChatContentPublisher.App.Views.Data;
using VRChatContentPublisher.App.Views.Data.Connect;
using VRChatContentPublisher.App.Views.Data.PublishTasks;
using VRChatContentPublisher.App.Views.Data.Settings;
using VRChatContentPublisher.App.Views.Dialogs;
using VRChatContentPublisher.App.Views.InAppNotifications;
using VRChatContentPublisher.App.Views.Settings;
using VRChatContentPublisher.Core;
using VRChatContentPublisher.Core.Services.App;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;

namespace VRChatContentPublisher.App;

public partial class App : Application
{
#pragma warning disable CS8600
#pragma warning disable CS8603
    public new static App Current => (App)Application.Current;
#pragma warning restore CS8603
#pragma warning restore CS8600

    private readonly IServiceProvider _serviceProvider = null!;

    public readonly AppWebImageLoader AsyncImageLoader;

    public App()
    {
        // Make Previewer happy
        var httpClient = new HttpClient();
        httpClient.AddUserAgent();

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        AsyncImageLoader = new AppWebImageLoader(new RemoteImageService(httpClient, memoryCache), memoryCache);
    }

    public App(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        AsyncImageLoader = _serviceProvider.GetRequiredService<AppWebImageLoader>();

        DataContext = _serviceProvider.GetRequiredService<AppViewModel>();
    }

    public override void Initialize()
    {
        ViewLocator.Register<BootstrapPageViewModel, BootstrapPage>();

        ViewLocator.Register<HomePageViewModel, HomePage>();
        ViewLocator.Register<SettingsPageViewModel, SettingsPage>();

        // HomePage Tabs
        ViewLocator.Register<HomeTasksPageViewModel, HomeTasksPage>();

        // Getting Started Pages
        ViewLocator.Register<GuideWelcomePageViewModel, GuideWelcomePage>();
        ViewLocator.Register<GuideSetupUnityPageViewModel, GuideSetupUnityPage>();
        ViewLocator.Register<GuideOpenConnectSettingsPageViewModel, GuideOpenConnectSettingsPage>();
        ViewLocator.Register<GuideConnectUnityPageViewModel, GuideConnectUnityPage>();

        // Settings Pages
        ViewLocator.Register<LoginPageViewModel, LoginPage>();
        ViewLocator.Register<LicensePageViewModel, LicensePage>();

        // Dialogs
        ViewLocator.Register<TwoFactorAuthDialogViewModel, TwoFactorAuthDialog>();
        ViewLocator.Register<RequestChallengeDialogViewModel, RequestChallengeDialog>();
        ViewLocator.Register<ExitAppDialogViewModel, ExitAppDialog>();
        ViewLocator.Register<StartupPortChangedDialogViewModel, StartupPortChangedDialog>();
        ViewLocator.Register<LoginWithCookiesDialogViewModel, LoginWithCookiesDialog>();
        ViewLocator.Register<UpdateAvailableDialogViewModel, UpdateAvailableDialog>();

        // Data
        ViewLocator.Register<PublishTaskManagerViewModel, PublishTaskManagerView>();
        ViewLocator.Register<InvalidSessionTaskManagerViewModel, InvalidSessionTaskManagerView>();
        ViewLocator.Register<PublishTaskViewModel, PublishTaskView>();
        ViewLocator.Register<PublishTaskManagerContainerViewModel, PublishTaskManagerContainerView>();

        ViewLocator.Register<RpcClientSessionViewModel, RpcClientSessionView>();
        ViewLocator.Register<UserSessionViewModel, UserSessionView>();
        
        ViewLocator.Register<UpdateDownloadProgressViewModel, UpdateDownloadProgressView>();

        // Settings Section
        ViewLocator.Register<ConnectSettingsViewModel, ConnectSettingsView>();
        ViewLocator.Register<NotificationSettingsViewModel, NotificationSettingsView>();
        ViewLocator.Register<AppearanceSettingsViewModel, AppearanceSettingsView>();
        ViewLocator.Register<HttpProxySettingsViewModel, HttpProxySettingsView>();
        ViewLocator.Register<AccountsSettingsViewModel, AccountsSettingsView>();
        ViewLocator.Register<SessionsSettingsViewModel, SessionsSettingsView>();
        ViewLocator.Register<AboutSettingsViewModel, AboutSettingsView>();
        ViewLocator.Register<DebugSettingsViewModel, DebugSettingsView>();
        ViewLocator.Register<UpdateSettingsViewModel, UpdateSettingsView>();

        // In App Notification
        ViewLocator.Register<PublicIpChangedInAppNotificationViewModel, PublicIpChangedInAppNotificationView>();
        ViewLocator.Register<UpdateAvailableAppNotificationViewModel, UpdateAvailableAppNotificationView>();
        ViewLocator.Register<UpdateProgressAppNotificationViewModel, UpdateProgressAppNotificationView>();

        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDeveloperTools();
#endif
        
        if (DataContext is not AppViewModel appViewModel)
            return;

        appViewModel.LoadCommand.Execute(null);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (!Design.IsDesignMode)
        {
            var appSettings = _serviceProvider.GetRequiredService<IWritableOptions<AppSettings>>();
            AppLocalizationService.Initialize(appSettings.Value.AppCulture);
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnOpenLogsFolderClicked(object? sender, EventArgs e)
    {
        if (DataContext is not AppViewModel appViewModel)
            return;

        var directoryPath = appViewModel.LogsFolderPath;
        if (!Directory.Exists(directoryPath))
            return;

        var directoryInfo = new DirectoryInfo(directoryPath);

        var topLevel =
            TopLevel.GetTopLevel((ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow);
        if (topLevel?.Launcher is { } launcher)
        {
            // Fire and forget
            _ = launcher.LaunchDirectoryInfoAsync(directoryInfo);
        }
    }
}