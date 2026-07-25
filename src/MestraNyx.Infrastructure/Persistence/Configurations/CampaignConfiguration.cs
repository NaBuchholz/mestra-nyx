using MestraNyx.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MestraNyx.Infrastructure.Persistence.Configurations;

public sealed class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("campaigns");

        builder.HasKey(campaign => campaign.Id);

        builder.Property(campaign => campaign.Id)
            .ValueGeneratedNever();

        builder.Property(campaign => campaign.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(campaign => campaign.Description)
            .HasMaxLength(2000);

        builder.Property(campaign => campaign.CoverImage)
            .HasMaxLength(500);

        builder.Property(campaign => campaign.OwnerId)
            .IsRequired();

        builder.HasIndex(campaign => campaign.OwnerId);
    }
}
