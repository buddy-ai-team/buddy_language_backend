namespace BuddyLanguage.Domain.Exceptions
{
    public class RecognizedTextIsEmptyException : DomainException
    {
        public RecognizedTextIsEmptyException()
            : base()
        {
        }

        public RecognizedTextIsEmptyException(string? message)
            : base(message)
        {
        }

        public RecognizedTextIsEmptyException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
