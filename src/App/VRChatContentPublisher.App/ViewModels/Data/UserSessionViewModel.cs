using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Services;
using VRChatContentPublisher.App.ViewModels.Pages;
using VRChatContentPublisher.Core.ContentPublishing.PublishTask.Services;
using VRChatContentPublisher.Core.UserSession;

namespace VRChatContentPublisher.App.ViewModels.Data;

public sealed partial class UserSessionViewModel(
    UserSessionService userSessionService,
    UserSessionManagerService userSessionManagerService,
    NavigationService navigationService,
    LoginPageViewModelFactory loginPageViewModelFactory) : ViewModelBase
{
    public string? UserId => userSessionService.UserId;
    public string UserNameOrEmail => userSessionService.UserNameOrEmail;
    public bool IsSessionRequiringReauthentication => userSessionService.State != UserSessionState.LoggedIn;
    public bool IsDefault => userSessionManagerService.IsDefaultSession(userSessionService);
    public bool CanSetDefault => !IsDefault;

    public string SetAsDefaultToolTip => CanSetDefault
        ? LangKeys.Pages_Settings_Accounts_Account_Item_Set_Default_Button_Tooltip
        : LangKeys.Pages_Settings_Accounts_Account_Item_Set_Default_Already_Set_Button_Tooltip;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RemoveButtonTooltip))]
    public partial bool CanRemove { get; private set; }

    public string RemoveButtonTooltip => CanRemove
        ? LangKeys.Pages_Settings_Accounts_Account_Item_Remove_Button_Tooltip
        : LangKeys
            .Pages_Settings_Accounts_Account_Item_Remove_Button_Cannot_Remove_Account_With_Existing_Tasks_Tooltip;

    public string? ProfilePictureUrl
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(userSessionService.CurrentUser?.ProfilePictureThumbnailUrl))
                return userSessionService.CurrentUser?.ProfilePictureThumbnailUrl;

            return userSessionService.CurrentUser?.AvatarThumbnailImageUrl;
        }
    }

    public string? DisplayName => userSessionService.CurrentUser?.DisplayName;

    [RelayCommand]
    private async Task Load()
    {
        userSessionService.StateChanged += OnUserSessionStateChanged;
        userSessionManagerService.DefaultSessionChanged += OnDefaultSessionChanged;

        await LoadCore();
    }

    private async ValueTask LoadCore()
    {
        if (userSessionService.State != UserSessionState.LoggedIn)
        {
            CanRemove = true;
            return;
        }

        try
        {
            var scope = await userSessionService.CreateOrGetSessionScopeAsync();
            var taskManager = scope.ServiceProvider.GetRequiredService<TaskManagerService>();

            CanRemove = taskManager.Tasks.Count == 0;
        }
        catch
        {
            // ignored
        }
    }

    [RelayCommand]
    private void Unload()
    {
        userSessionService.StateChanged -= OnUserSessionStateChanged;
        userSessionManagerService.DefaultSessionChanged -= OnDefaultSessionChanged;
    }

    [RelayCommand]
    private async Task SetAsDefault()
    {
        await userSessionManagerService.SetDefaultSessionAsync(userSessionService);
    }

    [RelayCommand]
    private async Task Remove()
    {
        await userSessionManagerService.RemoveSessionAsync(userSessionService);
    }

    [RelayCommand]
    private async Task Repair()
    {
        if (await userSessionService.TryRepairAsync())
            return;

        var fixPageViewModel = loginPageViewModelFactory.Create(
            navigationService.Navigate<SettingsPageViewModel>,
            navigationService.Navigate<SettingsPageViewModel>,
            userSessionService
        );

        navigationService.Navigate(fixPageViewModel);
    }

    [RelayCommand]
    private void ForceEnterInvalidState()
    {
        userSessionService.DebugForceEnterInvalidState();
    }

    private async void OnUserSessionStateChanged(object? sender, UserSessionState e)
    {
        await LoadCore();

        OnPropertyChanged(nameof(IsSessionRequiringReauthentication));
        OnPropertyChanged(nameof(UserId));
        OnPropertyChanged(nameof(UserNameOrEmail));
        OnPropertyChanged(nameof(ProfilePictureUrl));
        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(IsDefault));
        OnPropertyChanged(nameof(CanSetDefault));
        OnPropertyChanged(nameof(SetAsDefaultToolTip));
    }

    private void OnDefaultSessionChanged(object? sender, UserSessionService? e)
    {
        OnPropertyChanged(nameof(IsDefault));
        OnPropertyChanged(nameof(CanSetDefault));
        OnPropertyChanged(nameof(SetAsDefaultToolTip));
    }
}

public sealed class UserSessionViewModelFactory(
    UserSessionManagerService userSessionManagerService,
    NavigationService navigationService,
    LoginPageViewModelFactory loginPageViewModelFactory)
{
    public UserSessionViewModel Create(UserSessionService userSessionService)
    {
        return new UserSessionViewModel(
            userSessionService,
            userSessionManagerService,
            navigationService,
            loginPageViewModelFactory
        );
    }
}