using System.Net;
using System.Net.Http.Json;
using System.Text;
using MestraNyx.API.IntegrationTests;
using Microsoft.IdentityModel.Tokens;

namespace MestraNyx.API.IntegrationTests.Authentication;

public sealed class JwtBearerValidationTests
    : IClassFixture<MestraNyxApiFactory>
{
    private static SymmetricSecurityKey AlternateSigningKey { get; } = new(
        Encoding.UTF8.GetBytes("mestra-nyx-integration-tests-alternate-signing-key"));

    private readonly HttpClient _client;
    private readonly MestraNyxApiFactory _factory;

    public JwtBearerValidationTests(MestraNyxApiFactory factory)
    {
        _factory = factory;
        _factory.CampaignRepository.Reset();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCampaign_WithExpiredToken_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var token = TestJwtTokenFactory.CreateToken(
            subject: Guid.NewGuid().ToString(),
            notBefore: now.AddMinutes(-10),
            expires: now.AddMinutes(-5));

        _client.SetBearerToken(token);

        // Act
        var response = await PostCreateCampaignAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(_factory.CampaignRepository.AddedCampaign);
    }

    [Fact]
    public async Task CreateCampaign_WithWrongIssuer_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        var token = TestJwtTokenFactory.CreateToken(
            subject: Guid.NewGuid().ToString(),
            issuer: "untrusted-issuer");

        _client.SetBearerToken(token);

        // Act
        var response = await PostCreateCampaignAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(_factory.CampaignRepository.AddedCampaign);
    }

    [Fact]
    public async Task CreateCampaign_WithWrongAudience_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        var token = TestJwtTokenFactory.CreateToken(
            subject: Guid.NewGuid().ToString(),
            audience: "another-api");

        _client.SetBearerToken(token);

        // Act
        var response = await PostCreateCampaignAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(_factory.CampaignRepository.AddedCampaign);
    }

    [Fact]
    public async Task CreateCampaign_WithWrongSigningKey_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        var token = TestJwtTokenFactory.CreateToken(
            subject: Guid.NewGuid().ToString(),
            signingKey: AlternateSigningKey);

        _client.SetBearerToken(token);

        // Act
        var response = await PostCreateCampaignAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(_factory.CampaignRepository.AddedCampaign);
    }

    [Fact]
    public async Task CreateCampaign_WithMalformedToken_ReturnsUnauthorizedWithoutPersisting()
    {
        // Arrange
        _client.SetBearerToken("not-a-jwt");

        // Act
        var response = await PostCreateCampaignAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Null(_factory.CampaignRepository.AddedCampaign);
    }

    private Task<HttpResponseMessage> PostCreateCampaignAsync()
    {
        var request = new
        {
            Name = "Tiamat",
            System = "D&D 5e",
            Description = "A dragon epic campaign",
            TimePeriod = "735 BC"
        };

        return _client.PostAsJsonAsync("/api/campaigns", request);
    }
}
