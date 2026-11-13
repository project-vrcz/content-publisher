using VRChatContentPublisher.Core.UserSession;

namespace VRChatContentPublisher.Core.Events.UserSession;

public sealed class SessionStateChangedEvent(
    string? userId,
    string? displayName,
    string userNameOrEmail,
    UserSessionState sessionState,
    UserSessionState oldSessionState)
{
    public string? UserId => userId;
    public string UserNameOrEmail => userNameOrEmail;
    public string? DisplayName => displayName;

    public UserSessionState SessionState => sessionState;
    public UserSessionState OldSessionState => oldSessionState;
}