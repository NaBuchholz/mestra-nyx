using MestraNyx.Domain.Entities;
namespace MestraNyx.Domain.Interfaces;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Campaign>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Campaign>> GetPublicCampaignsAsync(int skip, int take, CancellationToken cancellationToken = default);
    Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default);

    Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

}