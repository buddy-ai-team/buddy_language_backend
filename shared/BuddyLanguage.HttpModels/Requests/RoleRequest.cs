using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.HttpModels.Requests;

public class RoleRequest
{
    [Required]
    public string Name { get; set; }
}