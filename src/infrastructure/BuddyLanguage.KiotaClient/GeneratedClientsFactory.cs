using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using OpenAI.GeneratedKiotaClient;

namespace BuddyLanguage.KiotaClient
{
    internal static class GeneratedClientsFactory
    {
        public static GeneratedOpenAiClient CreateGeneratedOpenAiClient(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient);
            var authProvider = new AnonymousAuthenticationProvider();
            var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
            return new GeneratedOpenAiClient(adapter);
        }
    }
}
