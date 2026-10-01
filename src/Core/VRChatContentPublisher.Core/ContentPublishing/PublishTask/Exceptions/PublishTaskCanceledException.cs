namespace VRChatContentPublisher.Core.ContentPublishing.PublishTask.Exceptions;

public class PublishTaskCanceledException(Exception? innerException)
    : Exception("Publishing task was canceled.", innerException);