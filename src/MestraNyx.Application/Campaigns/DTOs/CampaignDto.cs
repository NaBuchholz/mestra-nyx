using MestraNyx.Domain.Entities;

namespace MestraNyx.Application.Campaigns.DTOs;

// Usado para LEITURA - retornar dados para quem chamou a API
public record CampaignDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string System { get; init; } = string.Empty;
    public string? TimePeriod { get; init; }
    public string? CoverImage { get; init; }
    public bool IsPublic { get; init; }
    public bool IsActive { get; init; }
    public Guid OwnerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }

    // Factory Method: a responsabilidade de converter Campaign -> CampaignDto
    public static CampaignDto FromEntity(Campaign campaign) => new()
    {
        Id = campaign.Id,
        Name = campaign.Name,
        Description = campaign.Description,
        System = campaign.System,
        TimePeriod = campaign.TimePeriod,
        CoverImage = campaign.CoverImage,
        IsPublic = campaign.IsPublic,
        IsActive = campaign.IsActive,
        OwnerId = campaign.OwnerId,
        CreatedAt = campaign.CreatedAt,
        UpdatedAt = campaign.UpdatedAt
    };
}

// Usado para CRIAÇÃO - apenas os campos que fazem sentido vir de fora
public record CreateCampaignDto(
    string Name,
    string System,
    string? Description,
    string? TimePeriod
);

// Usado para ATUALIZAÇÃO - apenas os campos que podem ser alterados
public record UpdateCampaignDto(
    string Name,
    string? Description,
    string? TimePeriod,
    string? CoverImage,
    bool IsPublic,
    bool IsActive
);