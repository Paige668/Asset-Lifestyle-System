using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Services;

/// <summary>
/// Service for managing asset transactions (check-out/check-in)
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Check out an asset to a user
    /// </summary>
    Task<AssetTransaction> CheckOutAssetAsync(int assetId, string userName);

    /// <summary>
    /// Check in an asset from a user
    /// </summary>
    Task<AssetTransaction> CheckInAssetAsync(int assetId, string userName);

    /// <summary>
    /// Get transaction history
    /// </summary>
    Task<IEnumerable<AssetTransaction>> GetTransactionHistoryAsync();
}
