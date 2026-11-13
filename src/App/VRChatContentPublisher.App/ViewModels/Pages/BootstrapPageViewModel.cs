using Antelcat.I18N.Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Services;
using VRChatContentPublisher.App.Services.AppLifetime;
using VRChatContentPublisher.App.Services.Notification;
using VRChatContentPublisher.App.Services.Telemetry.PrivacyPolicy;
using VRChatContentPublisher.App.ViewModels.Pages.GettingStarted;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Services;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;
using VRChatContentPublisher.Core.UserSession;
using VRChatContentPublisher.TelemetryCore;

namespace VRChatContentPublisher.App.ViewModels.Pages;

public sealed partial class BootstrapPageViewModel(
    UserSessionManagerService sessionManagerService,
    NavigationService navigationService,
    IWritableOptions<AppSettings> appSettings,
    DesktopNotificationService desktopNotificationService,
    TaskRestoreService taskRestoreService,
    AppLifetimeService appLifetimeService,
    PrivacyPolicyService privacyPolicyService) : PageViewModelBase
{
    [ObservableProperty]
    public partial string LoadingText { get; private set; } =
        LangKeys.Pages_Bootstrap_Loading_Text_Initializing_Application;

    [RelayCommand]
    private async Task Load()
    {
        await appLifetimeService.WaitForHostStartedAsync();

        // Privacy policy consent check — must complete before anything else
        var latestPolicy = await privacyPolicyService.GetLatestPrivacyPolicyAsync();
        var storedVersion = TelemetrySettings.UserAgreementVersion;

        if (storedVersion is null || latestPolicy.Version > storedVersion)
        {
            navigationService.Navigate<PrivacyPolicyConsentPageViewModel>();
            return;
        }

        LoadingText = LangKeys.Pages_Bootstrap_Loading_Text_Logging_In_Accounts;
        await sessionManagerService.RestoreSessionsAsync();

        LoadingText = LangKeys.Pages_Bootstrap_Loading_Text_Restoring_Publish_Tasks;
        // Restore persisted publish tasks now that user sessions are available.
        await taskRestoreService.RestoreTasksAsync(sessionManagerService);

        // avoid call Navigate twice in same dispatcher loop in some case
        // (e.g. first launch of the app)
        Dispatcher.UIThread.Post(() =>
        {
            if (!appSettings.Value.SkipFirstSetup)
            {
                navigationService.Navigate<GuideWelcomePageViewModel>();
                return;
            }

            navigationService.Navigate<HomePageViewModel>(); 
        });
    }
}