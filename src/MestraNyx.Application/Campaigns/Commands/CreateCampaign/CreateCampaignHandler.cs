using MediatR;
using MestraNyx.Application.Campaigns.DTOs;
using MestraNyx.Domain.Entities;
using MestraNyx.Domain.Interfaces;

namespace MestraNyx.Application.Campaigns.Commands.CreateCampaign;

public sealed class CreateCampaignHandler : IRequestHandler<CreateCampaignCommand, CampaignDto>
{
    private readonly ICampaignRepository _campaignRepository;
    public CreateCampaignHandler(ICampaignRepository campaignRepository)
    {
        _campaignRepository = campaignRepository;
    }

    public async Task<CampaignDto> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var campaign = Campaign.Create(
            name: request.Name,
            ownerId: request.OwnerId,
            description: request.Description,
            system: request.System,
            timePeriod: request.TimePeriod);

        var savedCampaign = await _campaignRepository.AddAsync(
            campaign,
            cancellationToken);

        return CampaignDto.FromEntity(savedCampaign);
    }
}