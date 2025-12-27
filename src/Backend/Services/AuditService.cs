using ITAssetManager.Backend.Data;
using ITAssetManager.Backend.Models;

namespace ITAssetManager.Backend.Services;

/// <summary>
/// Implementation of audit logging service
/// </summary>
public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogActionAsync(string entityName, string action, string changes, string userName)
    {
        var auditLog = new AuditLog
        {
            EntityName = entityName,
            Action = action,
            Changes = changes,
            UserName = userName,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}
