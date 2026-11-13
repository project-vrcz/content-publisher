using Microsoft.Extensions.DependencyInjection;
using VRChatContentPublisher.App.Services;
using VRChatContentPublisher.App.Services.AppLifetime;
using VRChatContentPublisher.App.Services.Dialog;
using VRChatContentPublisher.App.Services.Notification;
using VRChatContentPublisher.App.Services.Notification.Sender;
using VRChatContentPublisher.App.Services.Telemetry.PrivacyPolicy;
using VRChatContentPublisher.App.Services.Update;
using VRChatContentPublisher.App.ViewModels;
using VRChatContentPublisher.App.ViewModels.Data;
using VRChatContentPublisher.App.ViewModels.Data.Connect;
using VRChatContentPublisher.App.ViewModels.Data.PublishTasks;
using VRChatContentPublisher.App.ViewModels.Dialogs;
using VRChatContentPublisher.App.ViewModels.InAppNotifications;
using VRChatContentPublisher.App.ViewModels.NetworkDiagnostic;
using VRChatContentPublisher.App.ViewModels.Pages;
using VRChatContentPublisher.App.ViewModels.Pages.GettingStarted;
using VRChatContentPublisher.App.ViewModels.Pages.HomeTab;
using VRChatContentPublisher.App.ViewModels.Pages.Settings;
using VRChatContentPublisher.App.ViewModels.Settings;
using VRChatContentPublisher.ConnectCore.Services.Connect.Challenge;
using VRChatContentPublisher.Core.AppServices;
using VRChatContentPublisher.IpcCore.Services;

namespace VRChatContentPublisher.App.Extensions;

public static class ServicesExtenstion
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<AppViewModel>();
        services.AddSingleton<AppWebImageLoader>();

        services.AddSingleton<AppLifetimeService>();
        services.AddSingleton<AppWindowService>();
        services.AddSingleton<IActivateWindowService>(s => s.GetRequiredService<AppWindowService>());
        services.AddSingleton<UriLauncherService>();

        services.AddSingleton<InAppNotificationService>();

        // Notification
        services.AddHostedService<AppNotificationHostedService>();
        services.AddSingleton<DesktopNotificationService>();

        // In App Notification
        services.AddTransient<PublicIpChangedInAppNotificationViewModelFactory>();
        services.AddTransient<UpdateAvailableAppNotificationViewModelFactory>();
        services.AddTransient<UpdateProgressAppNotificationViewModel>();
        services.AddTransient<SkipOnboardingOnFirstStartAppNotificationViewModel>();

        // Notification Senders
        services.AddHostedService<TaskFailedNotificationSenderService>();
        services.AddHostedService<TaskSuccessNotificationSenderService>();
        services.AddHostedService<PublicIpChangedNotificationSenderService>();
        services.AddHostedService<AppUpdateNotificationSender>();
        services.AddHostedService<SessionInvalidNotificationSender>();

        // Dialog
        services.AddSingleton<DialogService>();
        services.AddHostedService<DialogBackgroundService>();

        // Dialogs
        services.AddTransient<TwoFactorAuthDialogViewModelFactory>();
        services.AddTransient<RequestChallengeDialogViewModelFactory>();
        services.AddTransient<StartupPortChangedDialogViewModelFactory>();
        services.AddTransient<ExitAppDialogViewModel>();
        services.AddTransient<LoginWithCookiesDialogViewModelFactory>();
        services.AddTransient<UpdateAvailableDialogViewModelFactory>();
        services.AddTransient<PackageManagerUnavailableDialogViewModelFactory>();

        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<TaskErrorReportWindowViewModel>();
        services.AddTransient<NetworkDiagnosticWindowViewModel>();

        services.AddSingleton<NavigationService>();

        services.AddTransient<BootstrapPageViewModel>();

        services.AddSingleton<HomePageViewModel>();
        services.AddTransient<SettingsPageViewModel>();

        services.AddSingleton<HomeStatusBarViewModel>();

        // Data ViewModels
        services.AddTransient<UserSessionViewModelFactory>();
        services.AddTransient<PublishTaskViewModelFactory>();
        services.AddTransient<PublishTaskManagerViewModelFactory>();
        services.AddTransient<InvalidSessionTaskManagerViewModelFactory>();
        services.AddTransient<PublishTaskManagerContainerViewModelFactory>();

        services.AddTransient<RpcClientSessionViewModelFactory>();

        services.AddTransient<UpdateDownloadProgressViewModel>();

        // HomePage Tabs
        services.AddSingleton<HomeTasksPageViewModel>();

        // Getting Started Pages
        services.AddTransient<GuideWelcomePageViewModel>();
        services.AddTransient<GuideSetupUnityPageViewModel>();
        services.AddTransient<GuideOpenConnectSettingsPageViewModel>();
        services.AddTransient<GuideConnectUnityPageViewModel>();
        services.AddTransient<GuideBuildAndUploadPageViewModel>();

        // Settings Pages
        services.AddTransient<LoginPageViewModelFactory>();
        services.AddTransient<LicensePageViewModel>();

        // Settings Sections
        services.AddTransient<LazySettingsSectionViewModelFactory>();
        services.AddTransient<AccountsSettingsViewModel>();
        services.AddTransient<AppearanceSettingsViewModel>();
        services.AddTransient<ConnectSettingsViewModel>();
        services.AddTransient<NotificationSettingsViewModel>();
        services.AddTransient<HttpProxySettingsViewModel>();
        services.AddTransient<SessionsSettingsViewModel>();
        services.AddTransient<AboutSettingsViewModel>();
        services.AddTransient<DebugSettingsViewModel>();
        services.AddTransient<UpdateSettingsViewModel>();
        services.AddTransient<TelemetrySettingsViewModel>();

        // Connect Core
        services.AddSingleton<IRequestChallengeService, RequestChallengeService>();

        // Privacy Policy
        services.AddSingleton<PrivacyPolicyService>();
        services.AddHostedService<PrivacyPolicyPrefetchHostedService>();
        services.AddTransient<PrivacyPolicyConsentPageViewModel>();

        // Update Check
        services.AddSingleton<AppUpdateCheckService>();
        services.AddHostedService<AppUpdateCheckBackgroundService>();
        services.AddSingleton<AppUpdateService>();

#pragma warning disable EXTEXP0001
        services.AddHttpClient(nameof(AppUpdateService), client => client.Timeout = Timeout.InfiniteTimeSpan)
            .RemoveAllResilienceHandlers()
#pragma warning restore EXTEXP0001
            .ConfigurePrimaryHttpMessageHandler(serviceProvider => new SocketsHttpHandler
            {
                UseCookies = false,
                ConnectTimeout = TimeSpan.FromSeconds(10),
                Proxy = serviceProvider.GetRequiredService<AppWebProxy>(),
                PooledConnectionLifetime = TimeSpan.Zero,
                PooledConnectionIdleTimeout = TimeSpan.Zero
            });

        return services;
    }
}