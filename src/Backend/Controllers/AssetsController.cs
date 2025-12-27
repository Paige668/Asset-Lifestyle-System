using Microsoft.AspNetCore.Mvc;
using ITAssetManager.Backend.Services;
using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssets()
    {
        var assets = await _assetService.GetAllAssetsAsync();
        return Ok(assets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Asset>> GetAsset(int id)
    {
        var asset = await _assetService.GetAssetByIdAsync(id);
        if (asset == null) return NotFound();
        return asset;
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> CreateAsset(Asset asset)
    {
        try
        {
            var createdAsset = await _assetService.CreateAssetAsync(asset, "Admin"); // Mock user
            return CreatedAtAction(nameof(GetAsset), new { id = createdAsset.Id }, createdAsset);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsset(int id, Asset asset)
    {
        if (id != asset.Id) return BadRequest();

        try
        {
            await _assetService.UpdateAssetAsync(asset, "Admin"); // Mock user
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        try
        {
            await _assetService.DeleteAssetAsync(id, "Admin"); // Mock user
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
