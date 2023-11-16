using System.Runtime.Serialization;

namespace BuddyLanguage.Domain.Exceptions.Role;

public class PromptOfRoleNotDefinedException : DomainException
{
    public PromptOfRoleNotDefinedException()
    {
    }

    public PromptOfRoleNotDefinedException(string? message)
        : base(message)
    {
    }

    public PromptOfRoleNotDefinedException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
