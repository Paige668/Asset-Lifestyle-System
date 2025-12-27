using Microsoft.AspNetCore.Mvc;
using ITAssetManager.Backend.Services;
using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(int assetId, string userName)
    {
        try
        {
            var transaction = await _transactionService.CheckOutAssetAsync(assetId, userName);
            return Ok(transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(int assetId)
    {
        try
        {
            var transaction = await _transactionService.CheckInAssetAsync(assetId, "System");
            return Ok(transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetTransaction>>> GetHistory()
    {
        var history = await _transactionService.GetTransactionHistoryAsync();
        return Ok(history);
    }
}
