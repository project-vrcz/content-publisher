namespace VRChatContentPublisher.Core.ContentPublishing.PublishTask.Exceptions;

public class PublishingCanceledDueToSessionInvalidException(Exception? innerException)
    : Exception("Publishing was canceled due to invalid user session.", innerException);

public class PublishingCanceledException(Exception? innerException)
    : Exception("Publishing was canceled.", innerException);