using MestraNyx.Domain.Entities;
using MestraNyx.Domain.Interfaces;

namespace MestraNyx.Infrastructure.Persistence.Repositories;

public sealed class CampaignRepository : ICampaignRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CampaignRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Campaign?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<Campaign>> GetAllByOwnerIdAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<IReadOnlyList<Campaign>> GetPublicCampaignsAsync(
        int skip,
        int take,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async Task<Campaign> AddAsync(
        Campaign campaign,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Campaigns.AddAsync(campaign, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return campaign;
    }

    public Task UpdateAsync(
        Campaign campaign,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
