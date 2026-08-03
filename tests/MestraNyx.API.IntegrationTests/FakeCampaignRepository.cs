using MestraNyx.Domain.Entities;
using MestraNyx.Domain.Interfaces;

namespace MestraNyx.API.IntegrationTests;

internal sealed class FakeCampaignRepository : ICampaignRepository
{
    public Campaign? AddedCampaign { get; private set; } = null;
    public Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        AddedCampaign = campaign;
        return Task.FromResult(campaign);
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Campaign>> GetAllByOwnerIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Campaign?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Campaign>> GetPublicCampaignsAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
