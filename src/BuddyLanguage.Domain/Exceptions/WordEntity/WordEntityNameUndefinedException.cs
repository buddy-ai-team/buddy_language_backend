using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Exceptions.WordEntity
{
    public class WordEntityNameUndefinedException : DomainException
    {
        public WordEntityNameUndefinedException()
        {
        }

        public WordEntityNameUndefinedException(string? message)
            : base(message)
        {
        }

        public WordEntityNameUndefinedException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        protected WordEntityNameUndefinedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
