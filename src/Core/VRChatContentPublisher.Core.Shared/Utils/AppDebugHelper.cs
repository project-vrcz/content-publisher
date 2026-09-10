namespace VRChatContentPublisher.Core.Shared.Utils;

public static class AppDebugHelper
{
    public static bool IsDebugMode => IsDebugBuild() || IsDebugModeEnvironmentVariableSet();

    public static bool IsDebugBuild()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }

    private static bool IsDebugModeEnvironmentVariableSet()
    {
        var debugModeEnvVar = Environment.GetEnvironmentVariable("VRCP_DEBUG_MODE");
        return !string.IsNullOrEmpty(debugModeEnvVar) &&
               debugModeEnvVar.Equals("true", StringComparison.OrdinalIgnoreCase);
    }
}