namespace VRChatContentPublisher.Core.ContentPublishing.PublishTask.Exceptions;

public class BundleProcessingCanceledException(Exception? innerException) : 
    Exception("Bundle processing was canceled.", innerException);