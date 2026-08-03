using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MediatR;
using MestraNyx.Application.Campaigns.Commands.CreateCampaign;

namespace MestraNyx.API.Features.Campaigns.CreateCampaign;

internal static class CreateCampaignEndpoint
{
    public static IEndpointRouteBuilder MapCreateCampaignEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/campaigns", HandleAsync)
            .RequireAuthorization();

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        CreateCampaignRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var subjectClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(subjectClaim, out var ownerId))
        {
            return Results.Unauthorized();
        }

        var command = new CreateCampaignCommand(
            Name: request.Name,
            System: request.System,
            Description: request.Description,
            TimePeriod: request.TimePeriod,
            OwnerId: ownerId);

        var createdCampaign = await sender.Send(
            command,
            cancellationToken);

        return Results.Json(
            createdCampaign,
            statusCode: StatusCodes.Status201Created);
    }
}
