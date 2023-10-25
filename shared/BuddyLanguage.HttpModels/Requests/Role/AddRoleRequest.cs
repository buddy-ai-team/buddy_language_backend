using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.HttpModels.Requests.Role;

public class AddRoleRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Prompt { get; set; }
}