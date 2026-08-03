using System.Net.Http.Headers;

namespace MestraNyx.API.IntegrationTests.Authentication;

internal static class HttpClientAuthenticationExtensions
{
    internal static void SetBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
