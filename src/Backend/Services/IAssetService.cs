using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Services;

/// <summary>
/// Service for managing IT assets
/// </summary>
public interface IAssetService
{
    /// <summary>
    /// Get all assets
    /// </summary>
    Task<IEnumerable<Asset>> GetAllAssetsAsync();

    /// <summary>
    /// Get asset by ID
    /// </summary>
    Task<Asset?> GetAssetByIdAsync(int id);

    /// <summary>
    /// Create a new asset with validation
    /// </summary>
    Task<Asset> CreateAssetAsync(Asset asset, string userName);

    /// <summary>
    /// Update an existing asset
    /// </summary>
    Task UpdateAssetAsync(Asset asset, string userName);

    /// <summary>
    /// Delete an asset (with permission check)
    /// </summary>
    Task DeleteAssetAsync(int id, string userName);
}
