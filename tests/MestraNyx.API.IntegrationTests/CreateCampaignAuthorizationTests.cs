using System.Net;
using System.Net.Http.Json;

namespace MestraNyx.API.IntegrationTests;

public sealed class CreateCampaignAuthorizationTests
    : IClassFixture<MestraNyxApiFactory>
{
    private readonly HttpClient _client;

    public CreateCampaignAuthorizationTests(MestraNyxApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCampaign_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var request = new
        {
            Name = "Curse of Strahd",
            System = "D&D 5e",
            Description = "A gothic horror campaign",
            TimePeriod = "735 BC"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/campaigns",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}
