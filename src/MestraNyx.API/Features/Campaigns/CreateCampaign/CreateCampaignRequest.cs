namespace MestraNyx.API.Features.Campaigns.CreateCampaign;

internal sealed record CreateCampaignRequest(
    string Name,
    string? System,
    string? Description,
    string? TimePeriod);
