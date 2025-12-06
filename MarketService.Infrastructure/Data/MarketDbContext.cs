using MarketService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketService.Infrastructure.Data;

public class MarketDbContext : DbContext
{
    public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options) { }

    public DbSet<Market> Markets => Set<Market>();
    public DbSet<MarketOutcome> MarketOutcomes => Set<MarketOutcome>();
    public DbSet<MarketPosition> MarketPositions => Set<MarketPosition>();
    public DbSet<MarketResolution> MarketResolutions => Set<MarketResolution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Market>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Question)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(m => m.MarketPubKey)
                .IsRequired()
                .HasMaxLength(64);

            entity.HasIndex(m => m.MarketPubKey)
                .IsUnique();

            entity.HasMany(m => m.Outcomes)
                .WithOne(o => o.Market)
                .HasForeignKey(o => o.MarketId);

            entity.HasOne(m => m.Resolution)
                .WithOne(r => r.Market)
                .HasForeignKey<MarketResolution>(r => r.MarketId);
        });

        modelBuilder.Entity<MarketOutcome>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Label)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasIndex(o => new { o.MarketId, o.OutcomeIndex })
                .IsUnique();
        });

        modelBuilder.Entity<MarketPosition>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasIndex(p => new { p.UserId, p.MarketId });

            entity.Property(p => p.TxSignature)
                .HasMaxLength(128);
        });

        modelBuilder.Entity<MarketResolution>(entity =>
        {
            entity.HasKey(r => r.Id);
        });
    }
}

