using ITAssetManager.Backend.Data;
using ITAssetManager.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ITAssetManager.Backend.Services;

/// <summary>
/// Implementation of transaction service with state validation
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AssetTransaction> CheckOutAssetAsync(int assetId, string userName)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        
        if (asset == null)
        {
            throw new KeyNotFoundException($"Asset with ID {assetId} not found");
        }

        // Validation: Asset must be in stock to check out
        if (asset.Status != AssetStatus.InStock)
        {
            throw new InvalidOperationException($"Asset is not available for check-out. Current status: {asset.Status}");
        }

        // Update asset status
        asset.Status = AssetStatus.InUse;

        // Create transaction record
        var transaction = new AssetTransaction
        {
            AssetId = assetId,
            UserName = userName,
            Type = TransactionType.CheckOut,
            Date = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<AssetTransaction> CheckInAssetAsync(int assetId, string userName)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        
        if (asset == null)
        {
            throw new KeyNotFoundException($"Asset with ID {assetId} not found");
        }

        // Validation: Asset must be in use to check in
        if (asset.Status != AssetStatus.InUse)
        {
            throw new InvalidOperationException($"Asset is not currently checked out. Current status: {asset.Status}");
        }

        // Update asset status
        asset.Status = AssetStatus.InStock;

        // Create transaction record
        var transaction = new AssetTransaction
        {
            AssetId = assetId,
            UserName = userName,
            Type = TransactionType.CheckIn,
            Date = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<IEnumerable<AssetTransaction>> GetTransactionHistoryAsync()
    {
        return await _context.Transactions
            .Include(t => t.Asset)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }
}
