using Microsoft.EntityFrameworkCore;
using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetTransaction> Transactions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Asset>()
            .HasIndex(a => a.SerialNumber)
            .IsUnique();
    }
}
