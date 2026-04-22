using Antelcat.I18N.Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Messages.Connect;
using VRChatContentPublisher.App.Services.Dialog;
using VRChatContentPublisher.App.Services.NotificationSender;
using VRChatContentPublisher.App.ViewModels.Dialogs;
using VRChatContentPublisher.ConnectCore.Services.Connect.Challenge;
using VRChatContentPublisher.Core.Settings;
using VRChatContentPublisher.Core.Settings.Models;

namespace VRChatContentPublisher.App.Services;

public class RequestChallengeService(
    DialogService dialogService,
    RequestChallengeDialogViewModelFactory dialogViewModelFactory,
    AppNotificationService appNotificationService,
    IWritableOptions<AppSettings> appSettings) : IRequestChallengeService
{
    public Task RequestChallengeAsync(string code, string clientId, string identityPrompt, string clientName)
    {
        if (appSettings.Value.SendNotificationOnNewPairingRequest)
        {
            var title = LangKeys.Notifications_New_Pairing_Request_Title;
            var message = string.Format(
                I18NExtension.Translate(LangKeys.Notifications_New_Pairing_Request_Body_Template) ??
                "{0} is requesting to pair",
                clientName
            );
            _ = appNotificationService.SendNotificationAsync(title, message).AsTask();
        }

        Dispatcher.UIThread.Invoke(async () =>
        {
            await dialogService
                .ShowDialogAsync(
                    dialogViewModelFactory.Create(code, clientId, identityPrompt, clientName)).AsTask();
        });

        return Task.CompletedTask;
    }

    public Task CompleteChallengeAsync(string clientId)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            WeakReferenceMessenger.Default.Send(new ConnectChallengeCompletedMessage(clientId));
        });

        return Task.CompletedTask;
    }
}