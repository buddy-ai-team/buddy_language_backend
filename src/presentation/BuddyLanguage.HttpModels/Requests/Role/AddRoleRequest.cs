using System.ComponentModel.DataAnnotations;
using BuddyLanguage.Domain.Enumerations;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace BuddyLanguage.HttpModels.Requests.Role;

public class AddRoleRequest
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Prompt { get; set; }

    [Required]
    public RoleType RoleType { get; set; }
}
