using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Exceptions.TTS
{
    public class SpeechSynthesizingException : DomainException
    {
        public SpeechSynthesizingException()
        {
        }

        public SpeechSynthesizingException(string? message)
            : base(message)
        {
        }

        public SpeechSynthesizingException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected SpeechSynthesizingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
