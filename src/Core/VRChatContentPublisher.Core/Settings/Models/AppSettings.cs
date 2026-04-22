using VRChatContentPublisher.Core.Utils;

namespace VRChatContentPublisher.Core.Settings.Models;

public sealed class AppSettings
{
    public bool SkipFirstSetup { get; set; } = false;
    public bool UseRgbCyclingBackgroundMenu { get; set; } = false;

    public bool NotificationsEnabled { get; set; } = true;
    public bool SendNotificationOnStartupSessionRestoreFailed { get; set; } = true;
    public bool SendNotificationOnTaskFailed { get; set; } = true;
    public bool SendNotificationOnPublicIpChanged { get; set; } = true;
    public bool SendNotificationOnNewPairingRequest { get; set; } = true;

    public string? AppCulture { get; set; }

    public string ConnectInstanceName { get; set; } = RandomWordsUtils.GetRandomWords();
    public int RpcServerPort { get; set; } = 59328;

    public AppHttpProxySettings HttpProxySettings { get; set; } = AppHttpProxySettings.SystemProxy;
    public Uri? HttpProxyUri { get; set; }

    public AppTasksPageSortMode TasksPageSortMode { get; set; } = AppTasksPageSortMode.LatestFirst;

    public bool UseBorderlessWindow { get; set; } = true;

    public AppUpdateCheckMode UpdateCheckMode { get; set; } = AppUpdateCheckMode.AtStartAndBackground;
    public bool DownloadUpdateAtBackground { get; set; } = true;
    public bool ReceivePreviewUpdate { get; set; }
    public string? SkipVersion { get; set; }
}

public enum AppHttpProxySettings
{
    NoProxy = 0,
    SystemProxy = 1,
    CustomProxy = 2
}

public enum AppTasksPageSortMode
{
    LatestFirst = 0,
    OldestFirst = 1,
}

public enum AppUpdateCheckMode
{
    Manual,
    OnlyAtStart,
    AtStartAndBackground
}