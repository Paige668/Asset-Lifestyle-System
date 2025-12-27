namespace ITAssetManager.Backend.Services;

/// <summary>
/// Service for managing audit logs
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Log an action to the audit trail
    /// </summary>
    Task LogActionAsync(string entityName, string action, string changes, string userName);
}
