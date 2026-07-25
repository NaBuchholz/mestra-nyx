using FluentValidation;

namespace MestraNyx.Application.Campaigns.Commands.CreateCampaigns;

public sealed class CreateCampaignValidator : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignValidator()
    {
        RuleFor(campaign => campaign.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(campaign => campaign.Description)
            .MaximumLength(2000);

        RuleFor(campaign => campaign.OwnerId)
           .NotEmpty();
    }
}