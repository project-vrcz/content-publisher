using Antelcat.I18N.Avalonia;
using MessagePipe;
using Microsoft.Extensions.Hosting;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.Core.Events.UserSession;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;
using VRChatContentPublisher.Core.UserSession;

namespace VRChatContentPublisher.App.Services.Notification.Sender;

public class SessionInvalidNotificationSender(
    IWritableOptions<AppSettings> appSettings,
    DesktopNotificationService desktopNotificationService,
    ISubscriber<SessionStateChangedEvent> sessionStateChangedSubscriber) : IHostedService
{
    private IDisposable? _subscription;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscription = sessionStateChangedSubscriber.Subscribe(args =>
        {
            if (args.SessionState != UserSessionState.InvalidSession) return;
            if (!appSettings.Value.SendNotificationOnSessionInvalid) return;

            var title = string.Format(
                I18NExtension.Translate(LangKeys.Notifications_Session_Expired_Or_Invalid_Title_Template) ??
                "Session expired or invalid for user {0}",
                args.DisplayName ?? args.UserNameOrEmail);
            var description = I18NExtension.Translate(LangKeys.Notifications_Session_Expired_Or_Invalid_Description) ??
                              "All tasks in publish stage are cancelled, please retry after repair account.";

            _ = desktopNotificationService.SendNotificationAsync(title, description);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription?.Dispose();
        return Task.CompletedTask;
    }
}