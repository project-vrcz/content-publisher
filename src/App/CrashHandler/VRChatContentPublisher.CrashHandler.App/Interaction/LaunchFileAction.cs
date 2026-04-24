using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;

namespace VRChatContentPublisher.CrashHandler.App.Interaction;

public class LaunchFileAction : StyledElementAction
{
    public static readonly StyledProperty<string?> FilePathProperty =
        AvaloniaProperty.Register<LaunchFileAction, string?>(nameof(FilePath));

    public string? FilePath
    {
        get => GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var filePath = FilePath;
        if (string.IsNullOrEmpty(filePath))
        {
            return false;
        }

        if (!File.Exists(filePath))
        {
            return false;
        }

        var fileInfo = new FileInfo(filePath);

        var topLevel = TopLevel.GetTopLevel(sender as Visual);
        if (topLevel?.Launcher is { } launcher)
        {
            // Fire and forget
            _ = launcher.LaunchFileInfoAsync(fileInfo);
            return true;
        }

        return false;
    }
}
