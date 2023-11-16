namespace BuddyLanguage.Domain.Exceptions.WordEntity
{
    public class WordEntityNotFoundException : DomainException
    {
        public WordEntityNotFoundException()
        {
        }

        public WordEntityNotFoundException(string? message)
            : base(message)
        {
        }

        public WordEntityNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
