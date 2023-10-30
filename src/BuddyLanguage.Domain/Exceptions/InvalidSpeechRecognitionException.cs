using System.Runtime.Serialization;

namespace BuddyLanguage.Domain.Exceptions
{
    public class InvalidSpeechRecognitionException : DomainException
    {
        public InvalidSpeechRecognitionException()
        {
        }

        public InvalidSpeechRecognitionException(string? message)
            : base(message)
        {
        }

        public InvalidSpeechRecognitionException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected InvalidSpeechRecognitionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
