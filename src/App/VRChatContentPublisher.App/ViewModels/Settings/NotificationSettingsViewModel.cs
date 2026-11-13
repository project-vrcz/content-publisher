using CommunityToolkit.Mvvm.Input;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Services.Notification;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;

namespace VRChatContentPublisher.App.ViewModels.Settings;

public sealed partial class NotificationSettingsViewModel(
    IWritableOptions<AppSettings> appSettings,
    IWritableOptions<PublicIpStateStorage> publicIpStateStorage,
    DesktopNotificationService desktopNotificationService
) : ViewModelBase
{
    public bool IsDesktopNotificationSupported => desktopNotificationService.IsSupported;
    public bool IsNotificationInitializeFailed => desktopNotificationService.LastInitializationException != null;

    public string LastNotificationInitializationErrorMessage =>
        desktopNotificationService.LastInitializationException?.Message ?? string.Empty;

    public bool EnabledNotifications
    {
        get => appSettings.Value.NotificationsEnabled;
        set
        {
            if (appSettings.Value.NotificationsEnabled == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.NotificationsEnabled = value);
            OnPropertyChanged();
        }
    }

    public bool SendNotificationOnSessionInvalid
    {
        get => appSettings.Value.SendNotificationOnSessionInvalid;
        set
        {
            if (appSettings.Value.SendNotificationOnSessionInvalid == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.SendNotificationOnSessionInvalid = value);
            OnPropertyChanged();
        }
    }

    public bool SendNotificationOnTaskFailed
    {
        get => appSettings.Value.SendNotificationOnTaskFailed;
        set
        {
            if (appSettings.Value.SendNotificationOnTaskFailed == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.SendNotificationOnTaskFailed = value);
            OnPropertyChanged();
        }
    }

    public bool SendNotificationOnTaskSucceeded
    {
        get => appSettings.Value.SendNotificationOnTaskSucceeded;
        set
        {
            if (appSettings.Value.SendNotificationOnTaskSucceeded == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.SendNotificationOnTaskSucceeded = value);
            OnPropertyChanged();
        }
    }

    public bool EnablePublicIpMonitor
    {
        get => appSettings.Value.EnablePublicIpMonitor;
        set
        {
            if (appSettings.Value.EnablePublicIpMonitor == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.EnablePublicIpMonitor = value);

            if (!value)
            {
                publicIpStateStorage.Update(storage =>
                {
                    storage.LastPublicIp = null;
                    storage.LastPreviousIp = null;
                    storage.LastCheckedAtUtc = null;
                    storage.LastChangedAtUtc = null;
                    storage.IsWarningDismissed = true;
                });
            }

            OnPropertyChanged();
        }
    }

    public bool SendNotificationOnPublicIpChanged
    {
        get => appSettings.Value.SendNotificationOnPublicIpChanged;
        set
        {
            if (appSettings.Value.SendNotificationOnPublicIpChanged == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.SendNotificationOnPublicIpChanged = value);
            OnPropertyChanged();
        }
    }

    public bool SendNotificationOnNewPairingRequest
    {
        get => appSettings.Value.SendNotificationOnNewPairingRequest;
        set
        {
            if (appSettings.Value.SendNotificationOnNewPairingRequest == value)
                return;

            OnPropertyChanging();
            appSettings.Update(settings => settings.SendNotificationOnNewPairingRequest = value);
            OnPropertyChanged();
        }
    }

    [RelayCommand]
    private async Task SendTestNotification()
    {
        await desktopNotificationService.SendNotificationAsync(
            LangKeys.Notifications_Settings_Test_Notification_Title,
            LangKeys.Notifications_Settings_Test_Notification_Body
        );
    }
}