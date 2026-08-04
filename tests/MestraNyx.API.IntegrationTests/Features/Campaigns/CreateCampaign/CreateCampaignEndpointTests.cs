using System.Net;
using System.Net.Http.Json;
using MestraNyx.API.IntegrationTests.Authentication;
using MestraNyx.Application.Campaigns.DTOs;
using Microsoft.AspNetCore.Http;

namespace MestraNyx.API.IntegrationTests.Features.Campaigns.CreateCampaign;

public sealed class CreateCampaignEndpointTests
    : IClassFixture<MestraNyxApiFactory>
{
    private readonly HttpClient _client;
    private readonly MestraNyxApiFactory _factory;

    public CreateCampaignEndpointTests(MestraNyxApiFactory factory)
    {
        _factory = factory;
        factory.CampaignRepository.Reset();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCampaign_WithValidRequest_ReturnsCreatedCampaign()
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
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(
            "application/json",
            response.Content.Headers.ContentType?.MediaType);

        var campaign = await response.Content.ReadFromJsonAsync<CampaignDto>();

        Assert.NotNull(campaign);
        Assert.NotEqual(Guid.Empty, campaign.Id);
        Assert.Equal(request.Name, campaign.Name);
        Assert.Equal(request.System, campaign.System);
        Assert.Equal(request.Description, campaign.Description);
        Assert.Equal(request.TimePeriod, campaign.TimePeriod);
        Assert.Equal(ownerId, campaign.OwnerId);
    }

    [Fact]
    public async Task CreateCampaign_WithInvalidRequest_ReturnsValidationProblem()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var token = TestJwtTokenFactory.CreateToken(ownerId.ToString());

        _client.SetBearerToken(token);

        var request = new
        {
            Name = string.Empty,
            System = "D&D 5e",
            Description = "A gothic horror campaign",
            TimePeriod = "735 BC"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/campaigns",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.True(problem.Errors.ContainsKey("name"));
        Assert.NotEmpty(problem.Errors["name"]);

        var campaign = _factory.CampaignRepository.AddedCampaign;
        Assert.Null(campaign);
    }
}
