using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITAssetManager.Backend.Data;
using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransactionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(int assetId, string userName)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null) return NotFound("Asset not found");
        if (asset.Status != AssetStatus.InStock) return BadRequest("Asset not in stock");

        // Update Asset
        asset.Status = AssetStatus.InUse;
        
        // Record Transaction
        var transaction = new AssetTransaction
        {
            AssetId = assetId,
            UserName = userName,
            Type = TransactionType.CheckOut
        };
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return Ok(transaction);
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(int assetId)
    {
        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null) return NotFound("Asset not found");
        if (asset.Status != AssetStatus.InUse) return BadRequest("Asset not in use");

        // Update Asset
        asset.Status = AssetStatus.InStock;

        // Record Transaction
        var transaction = new AssetTransaction
        {
            AssetId = assetId,
            UserName = "System", // Or last user
            Type = TransactionType.CheckIn
        };
        _context.Transactions.Add(transaction);

        await _context.SaveChangesAsync();
        return Ok(transaction);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetTransaction>>> GetHistory()
    {
        return await _context.Transactions.Include(t => t.Asset).OrderByDescending(t => t.Date).ToListAsync();
    }
}
