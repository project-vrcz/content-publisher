using VRChatContentPublisher.App.Models.Update;

namespace VRChatContentPublisher.App.Services.Update;

public union AppUpdateServiceState(
    AppUpdateServiceState.Idle,
    AppUpdateServiceState.Downloading,
    AppUpdateServiceState.DownloadError,
    AppUpdateServiceState.IntegrityCheckFailed,
    AppUpdateServiceState.WaitingForInstall,
    AppUpdateServiceState.InstallError
)
{
    public sealed class Idle;

    public sealed class Downloading
    {
        public required AppUpdateInformation UpdateInformation { get; init; }
        public required string PathToDownloadedFile { get; init; }

        public double BytesPerSecondSpeed { get; set; }
        public long TotalFileSize { get; set; }
        public long DownloadedFileSize { get; set; }

        public required CancellationTokenSource DownloadCancellationTokenSource { get; init; }
    }

    public sealed class DownloadError
    {
        public required AppUpdateInformation UpdateInformation { get; init; }
        public required Exception DownloadErrorException { get; init; }
    }

    public sealed class IntegrityCheckFailed
    {
        public required AppUpdateInformation UpdateInformation { get; init; }

        public required string DownloadedFileSha256 { get; init; }
        public required string ExceptedFileSha256 { get; init; }
    }

    public sealed class WaitingForInstall
    {
        public required AppUpdateInformation UpdateInformation { get; init; }

        public required string PathToDownloadedFile { get; init; }
    }

    public sealed class InstallError
    {
        public required AppUpdateInformation UpdateInformation { get; init; }

        public required Exception InstallErrorException { get; init; }
        public required string PathToDownloadedFile { get; init; }
    }
}