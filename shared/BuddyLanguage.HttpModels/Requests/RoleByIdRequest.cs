using System.ComponentModel.DataAnnotations;

namespace BuddyLanguage.HttpModels.Requests;

public class RoleByIdRequest
{
    [Required]
    public Guid Id { get; set; }
}