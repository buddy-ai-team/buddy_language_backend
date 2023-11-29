using System.Text.Json.Serialization;

namespace BuddyLanguage.WebApi.Filters;

public class AuthUser
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("language_code")]
    public string LanguageCode { get; set; } = "en-US";

    [JsonPropertyName("is_premium")]
    public bool IsPremium { get; set; } = false;
}
