using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.HttpModels.Requests.Role;

public class RoleByIdRequest
{
    [Required]
    public Guid Id { get; set; }
}
