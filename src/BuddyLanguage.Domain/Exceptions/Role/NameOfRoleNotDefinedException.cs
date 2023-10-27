using System.Runtime.Serialization;

namespace BuddyLanguage.Domain.Exceptions.Role;

public class NameOfRoleNotDefinedException : DomainException
{
    public NameOfRoleNotDefinedException()
    {
    }

    public NameOfRoleNotDefinedException(string? message)
        : base(message)
    {
    }

    public NameOfRoleNotDefinedException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    protected NameOfRoleNotDefinedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
