using System.Runtime.Serialization;

namespace BuddyLanguage.Domain.Exceptions.AzureService;

public class SpeechNotRecognizedException : DomainException
{
    public SpeechNotRecognizedException()
    {
    }

    public SpeechNotRecognizedException(string? message)
        : base(message)
    {
    }

    public SpeechNotRecognizedException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected SpeechNotRecognizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
