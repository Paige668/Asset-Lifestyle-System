namespace ITAssetManager.Backend.Models;

public enum AssetStatus
{
    InStock = 0,
    InUse = 1,
    Retired = 2
}

public class Asset
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string SerialNumber { get; set; }
    public AssetStatus Status { get; set; } = AssetStatus.InStock;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum TransactionType
{
    CheckOut = 0,
    CheckIn = 1
}

public class AssetTransaction
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public Asset? Asset { get; set; }
    public required string UserName { get; set; } // Simplified for Demo (RBAC via claim)
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
}

public class AuditLog
{
    public int Id { get; set; }
    public required string EntityName { get; set; }
    public required string Action { get; set; } // "Create", "Update", "Delete"
    public required string Changes { get; set; } // JSON or simple string
    public required string UserName { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
