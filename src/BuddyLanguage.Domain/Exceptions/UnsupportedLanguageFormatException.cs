namespace BuddyLanguage.Domain.Exceptions
{
    public class UnsupportedLanguageFormatException : DomainException
    {
        public UnsupportedLanguageFormatException()
            : base()
        {
        }

        public UnsupportedLanguageFormatException(string? message)
            : base(message)
        {
        }

        public UnsupportedLanguageFormatException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
