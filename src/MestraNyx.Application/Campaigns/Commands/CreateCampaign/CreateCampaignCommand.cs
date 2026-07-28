using MediatR;
using MestraNyx.Application.Campaigns.DTOs;

namespace MestraNyx.Application.Campaigns.Commands.CreateCampaign;

public sealed record CreateCampaignCommand(
    string Name,
    string? System,
    string? Description,
    string? TimePeriod,
    Guid OwnerId
) : IRequest<CampaignDto>;
