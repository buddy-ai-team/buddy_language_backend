namespace BuddyLanguage.Domain.Exceptions
{
    public class InvalidTextToSpeechException : DomainException
    {
        public InvalidTextToSpeechException()
        {
        }

        public InvalidTextToSpeechException(string? message)
            : base(message)
        {
        }

        public InvalidTextToSpeechException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
