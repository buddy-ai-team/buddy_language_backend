using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.HttpModels.Requests;

public class UpdateRoleRequest
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Prompt { get; set; }
}