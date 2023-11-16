using System.Runtime.Serialization;

namespace BuddyLanguage.Domain.Exceptions.Role;

public class RoleNotFoundException : DomainException
{
    public RoleNotFoundException()
    {
    }

    public RoleNotFoundException(string? message)
        : base(message)
    {
    }

    public RoleNotFoundException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
