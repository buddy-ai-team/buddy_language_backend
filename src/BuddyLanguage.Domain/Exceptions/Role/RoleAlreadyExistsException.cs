using System.Runtime.Serialization;

namespace BuddyLanguage.Domain.Exceptions.Role;

public class RoleAlreadyExistsException : DomainException
{
    public RoleAlreadyExistsException()
    {
    }

    public RoleAlreadyExistsException(string? message)
        : base(message)
    {
    }

    public RoleAlreadyExistsException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected RoleAlreadyExistsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
