using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Exceptions.User
{
    public class UserNotFoundException : DomainException
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string? message)
            : base(message)
        {
        }

        public UserNotFoundException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
