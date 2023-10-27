using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

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

        protected WordEntityNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
