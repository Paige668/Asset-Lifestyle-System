using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ITAssetManager.Backend.Data;
using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetLogs()
    {
        return await _context.AuditLogs.OrderByDescending(l => l.Timestamp).Take(100).ToListAsync();
    }
}
