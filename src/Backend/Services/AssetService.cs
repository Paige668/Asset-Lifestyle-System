using ITAssetManager.Backend.Data;
using ITAssetManager.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetManager.Backend.Services;

/// <summary>
/// Implementation of asset management service with business logic
/// </summary>
public class AssetService : IAssetService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public AssetService(AppDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        return await _context.Assets.ToListAsync();
    }

    public async Task<Asset?> GetAssetByIdAsync(int id)
    {
        return await _context.Assets.FindAsync(id);
    }

    public async Task<Asset> CreateAssetAsync(Asset asset, string userName)
    {
        // Validation: Name cannot be empty
        if (string.IsNullOrWhiteSpace(asset.Name))
        {
            throw new ArgumentException("Asset name cannot be empty", nameof(asset.Name));
        }

        // Validation: Serial number must be unique
        var existingAsset = await _context.Assets
            .FirstOrDefaultAsync(a => a.SerialNumber == asset.SerialNumber);
        
        if (existingAsset != null)
        {
            throw new InvalidOperationException($"An asset with serial number '{asset.SerialNumber}' already exists");
        }

        asset.CreatedAt = DateTime.UtcNow;
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Log the creation
        await _auditService.LogActionAsync(
            "Asset",
            "Create",
            $"Created asset: {asset.Name} (SN: {asset.SerialNumber})",
            userName
        );

        return asset;
    }

    public async Task UpdateAssetAsync(Asset asset, string userName)
    {
        _context.Entry(asset).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(
            "Asset",
            "Update",
            $"Updated asset ID {asset.Id}: {asset.Name}",
            userName
        );
    }

    public async Task DeleteAssetAsync(int id, string userName)
    {
        // Permission check: Only admin can delete
        if (!userName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Only administrators can delete assets");
        }

        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
        {
            throw new KeyNotFoundException($"Asset with ID {id} not found");
        }

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(
            "Asset",
            "Delete",
            $"Deleted asset ID {id}: {asset.Name}",
            userName
        );
    }
}
