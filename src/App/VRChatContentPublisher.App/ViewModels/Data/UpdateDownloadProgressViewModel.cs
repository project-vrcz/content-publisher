using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VRChatContentPublisher.App.Localization;
using VRChatContentPublisher.App.Models.Update;
using VRChatContentPublisher.App.Services.AppLifetime;
using VRChatContentPublisher.App.Services.Dialog;
using VRChatContentPublisher.App.Services.Update;
using VRChatContentPublisher.App.ViewModels.Dialogs;

namespace VRChatContentPublisher.App.ViewModels.Data;

public sealed partial class UpdateDownloadProgressViewModel(
    AppUpdateService appUpdateService,
    DialogService dialogService,
    AppLifetimeService lifetimeService,
    IServiceProvider serviceProvider
) : ViewModelBase
{
    [ObservableProperty] public partial bool ShowWhatNewsButton { get; set; } = true;

    private AppUpdateServiceState.Downloading? DownloadingState =>
        appUpdateService.UpdateState.Value as AppUpdateServiceState.Downloading;

    public long TotalFileSize => DownloadingState?.TotalFileSize ?? 1;
    public long DownloadedFileSize => DownloadingState?.DownloadedFileSize ?? 0;
    public string DownloadPrecent => ((double)DownloadedFileSize / TotalFileSize * 100).ToString("N") + "%";

    public double MebibytePerSecondSpeed => DownloadingState is not null
        ? DownloadingState.BytesPerSecondSpeed / 1.048576e+6
        : 0;

    public string? TargetVersion => appUpdateService.UpdateInformation?.Version;

    public bool IsDownloading => appUpdateService.UpdateState is AppUpdateServiceState.Downloading;
    public bool IsWaitingForInstall => appUpdateService.UpdateState is AppUpdateServiceState.WaitingForInstall;
    public bool IsUpdateInstallationSupported => appUpdateService.IsAppUpdateSupported();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanInstallUpdate))]
    [NotifyPropertyChangedFor(nameof(InstallUpdateButtonText))]
    public partial bool IsSafeToShutdown { get; private set; }

    public bool CanInstallUpdate => IsSafeToShutdown && IsUpdateInstallationSupported;

    public bool IsError =>
        appUpdateService.UpdateState is AppUpdateServiceState.DownloadError
            or AppUpdateServiceState.IntegrityCheckFailed
            or AppUpdateServiceState.InstallError;

    public string ErrorTitleText =>
        appUpdateService.UpdateState is AppUpdateServiceState.InstallError
            ? LangKeys.Common_Views_Update_Progress_View_Update_Install_Failed_Title
            : LangKeys.Common_Views_Update_Progress_View_Update_Download_Failed_Title;

    public string? UpdateError => appUpdateService.LastException?.Message;

    public string InstallUpdateButtonText => IsSafeToShutdown
        ? LangKeys.Common_Views_Update_Progress_View_Install_Update_Button
        : LangKeys.Common_Views_Update_Progress_View_Install_Update_Disabled_Due_To_Uncompleted_Tasks;

    private readonly DispatcherTimer _updateProgressTimer =
        new(TimeSpan.FromMilliseconds(500), DispatcherPriority.Background, Dispatcher.UIThread);

    [RelayCommand]
    private void Load()
    {
        IsSafeToShutdown = lifetimeService.IsSafeToShutdown();

        appUpdateService.OnUpdateStateChanged += OnUpdateStateChanged;
        _updateProgressTimer.Tick += OnUpdateProgressTimerTick;
        lifetimeService.IsSafeToShutdownChanged += IsSafeToShutdownChanged;
        UpdateTimerEnabled();
    }

    [RelayCommand]
    private void Unload()
    {
        appUpdateService.OnUpdateStateChanged -= OnUpdateStateChanged;
        _updateProgressTimer.Tick -= OnUpdateProgressTimerTick;
        lifetimeService.IsSafeToShutdownChanged -= IsSafeToShutdownChanged;
        _updateProgressTimer.Stop();
    }

    [RelayCommand]
    private async Task CancelUpdate()
    {
        await appUpdateService.CancelUpdateAsync();
    }

    [RelayCommand]
    private async Task RetryDownloadUpdate()
    {
        await appUpdateService.RetryUpdateAsync();
    }

    [RelayCommand]
    private async Task InstallUpdate()
    {
        try
        {
            await appUpdateService.InstallUpdateAsync();
        }
        catch
        {
            // ignored
        }
    }

    [RelayCommand]
    private async Task ShowWhatNews()
    {
        if (appUpdateService.UpdateInformation is not { } update)
            return;

        var updateAvailableDialogViewModelFactory =
            serviceProvider.GetRequiredService<UpdateAvailableDialogViewModelFactory>();
        await dialogService.ShowDialogAsync(updateAvailableDialogViewModelFactory.Create(update));
    }

    private void OnUpdateProgressTimerTick(object? sender, EventArgs e)
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        OnPropertyChanged(nameof(TotalFileSize));
        OnPropertyChanged(nameof(DownloadedFileSize));
        OnPropertyChanged(nameof(DownloadPrecent));
        OnPropertyChanged(nameof(MebibytePerSecondSpeed));
    }

    private void OnUpdateStateChanged(object? sender, AppUpdateServiceState e)
    {
        OnPropertyChanged(nameof(TargetVersion));

        OnPropertyChanged(nameof(IsDownloading));
        OnPropertyChanged(nameof(IsWaitingForInstall));
        OnPropertyChanged(nameof(IsError));
        OnPropertyChanged(nameof(ErrorTitleText));
        OnPropertyChanged(nameof(UpdateError));

        UpdateProgress();
        UpdateTimerEnabled();
    }

    private void UpdateTimerEnabled()
    {
        _updateProgressTimer.IsEnabled = appUpdateService.UpdateState is AppUpdateServiceState.Downloading;
    }

    private void IsSafeToShutdownChanged(object? sender, bool e)
    {
        IsSafeToShutdown = e;
    }
}