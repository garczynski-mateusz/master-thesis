using System.Net.Http;

namespace CateringBackend.CrossTests.Utilities
{
    public static class HttpClientExtensions
    {
        public static void RemoveAuthorization(this HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Remove("api-key");
        }
    }
}
