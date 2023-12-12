using BuddyLanguage.Domain.Interfaces;

namespace BuddyLanguage.AzureServices;

public class OggOpusToPcmConverterAzureFunction : IOggOpusToPcmConverter
{
    private readonly HttpClient _httpClient;
    private readonly string _functionUrl = "???";

    public OggOpusToPcmConverterAzureFunction(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<byte[]> ConvertOggToPcm(byte[] oggData)
    {
        using var content = new ByteArrayContent(oggData);
        var response = await _httpClient.PostAsync(_functionUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Error calling Azure function: {response.StatusCode}, Details: {errorContent}");
        }

        return await response.Content.ReadAsByteArrayAsync();
    }
}
