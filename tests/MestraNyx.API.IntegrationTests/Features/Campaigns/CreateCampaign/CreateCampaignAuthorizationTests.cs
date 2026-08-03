using System.Net;
using System.Net.Http.Json;
using MestraNyx.API.IntegrationTests;
using MestraNyx.API.IntegrationTests.Authentication;

namespace MestraNyx.API.IntegrationTests.Features.Campaigns.CreateCampaign;

public sealed class CreateCampaignAuthorizationTests
    : IClassFixture<MestraNyxApiFactory>
{
    private readonly HttpClient _client;
    private readonly MestraNyxApiFactory _factory;

    public CreateCampaignAuthorizationTests(MestraNyxApiFactory factory)
    {
        _factory = factory;
        _factory.CampaignRepository.Reset();
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

    [Fact]
    public async Task CreateCampaign_WithValidToken_UsesSubjectAsOwnerId()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        var token = TestJwtTokenFactory.CreateToken(ownerId.ToString());

        _client.SetBearerToken(token);

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
            HttpStatusCode.Created,
            response.StatusCode);

        var addedCampaign = _factory.CampaignRepository.AddedCampaign;

        Assert.NotNull(addedCampaign);
        Assert.Equal(ownerId, addedCampaign.OwnerId);
    }

    [Fact]
    public async Task CreateCampaign_WithOwnerIdInBody_UsesAuthenticatedSubjectAsOwnerId()
    {
        // Arrange
        var authenticatedOwnerId = Guid.NewGuid();

        var requestedOwnerId = Guid.NewGuid();

        var token = TestJwtTokenFactory.CreateToken(authenticatedOwnerId.ToString());

        _client.SetBearerToken(token);

        var request = new
        {
            Name = "Curse",
            System = "D&D 5e",
            Description = "A gothic horror campaign",
            TimePeriod = "735 BC",
            OwnerId = requestedOwnerId.ToString()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/campaigns", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var addedCampaign = _factory.CampaignRepository.AddedCampaign;

        Assert.NotNull(addedCampaign);
        Assert.Equal(authenticatedOwnerId, addedCampaign.OwnerId);
    }

    [Fact]
    public async Task CreateCampaign_WithNonGuidSubject_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        var invalidSubject = "not-a-guid";

        var token = TestJwtTokenFactory.CreateToken(invalidSubject);

        _client.SetBearerToken(token);

        var request = new
        {
            Name = "Tiamat",
            System = "D&D 5e",
            Description = "A dragon epic campaign",
            TimePeriod = "735 BC"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/campaigns", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var addedCampaign = _factory.CampaignRepository.AddedCampaign;

        Assert.Null(addedCampaign);
    }

    [Fact]
    public async Task CreateCampaign_WithoutSubject_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        var token = TestJwtTokenFactory.CreateToken();

        _client.SetBearerToken(token);

        var request = new
        {
            Name = "Tiamat",
            System = "D&D 5e",
            Description = "A dragon epic campaign",
            TimePeriod = "735 BC"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/campaigns", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var addedCampaign = _factory.CampaignRepository.AddedCampaign;

        Assert.Null(addedCampaign);
    }
}
