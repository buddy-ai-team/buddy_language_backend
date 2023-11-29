using System.Text.Json.Serialization;

namespace BuddyLanguage.WebApi.Filters;

public class InitData
{
    [JsonPropertyName("auth_date")]
    public required int AuthDateRaw { get; set; }

    [JsonPropertyName("hash")]
    public required string Hash { get; set; }

    [JsonPropertyName("query_id")]
    public required string QueryId { get; set; }

    [JsonPropertyName("user")]
    public required AuthUser User { get; set; }

    // Convert AuthDateRaw to DateTime
    public DateTime AuthDate => DateTimeOffset.FromUnixTimeSeconds(AuthDateRaw).DateTime;
}
