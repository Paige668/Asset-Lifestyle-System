using Microsoft.EntityFrameworkCore;

namespace ITAssetManager.Tests.Services;

/// <summary>
/// Unit tests for AssetService covering key business scenarios
/// </summary>
public class AssetServiceTests
{
    private readonly Mock<IAuditService> _mockAuditService;
    private readonly AppDbContext _context;
    private readonly AssetService _assetService;

    public AssetServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _mockAuditService = new Mock<IAuditService>();
        _assetService = new AssetService(_context, _mockAuditService.Object);
    }

    /// <summary>
    /// Test 1: Asset creation with empty name should throw ArgumentException
    /// </summary>
    [Fact]
    public async Task CreateAsset_WithEmptyName_ThrowsValidationException()
    {
        // Arrange
        var asset = new Asset
        {
            Name = "",
            SerialNumber = "SN-001"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _assetService.CreateAssetAsync(asset, "Admin")
        );

        exception.Message.Should().Contain("name cannot be empty");
    }

    /// <summary>
    /// Test 2: Asset creation with duplicate serial number should throw InvalidOperationException
    /// </summary>
    [Fact]
    public async Task CreateAsset_WithDuplicateSerialNumber_ThrowsValidationException()
    {
        // Arrange
        var existingAsset = new Asset
        {
            Name = "Laptop 1",
            SerialNumber = "SN-DUPLICATE",
            CreatedAt = DateTime.UtcNow
        };
        _context.Assets.Add(existingAsset);
        await _context.SaveChangesAsync();

        var duplicateAsset = new Asset
        {
            Name = "Laptop 2",
            SerialNumber = "SN-DUPLICATE"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _assetService.CreateAssetAsync(duplicateAsset, "Admin")
        );

        exception.Message.Should().Contain("already exists");
    }

    /// <summary>
    /// Test 3: Check-out when asset is Available should succeed
    /// </summary>
    [Fact]
    public async Task CheckOut_WhenAssetIsAvailable_Succeeds()
    {
        // Arrange
        var transactionService = new TransactionService(_context);
        var asset = new Asset
        {
            Name = "Monitor",
            SerialNumber = "MON-001",
            Status = AssetStatus.InStock,
            CreatedAt = DateTime.UtcNow
        };
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Act
        var transaction = await transactionService.CheckOutAssetAsync(asset.Id, "TestUser");

        // Assert
        transaction.Should().NotBeNull();
        transaction.Type.Should().Be(TransactionType.CheckOut);
        transaction.UserName.Should().Be("TestUser");
    }

    /// <summary>
    /// Test 4: Check-out updates asset status to InUse
    /// </summary>
    [Fact]
    public async Task CheckOut_UpdatesAssetStatusToInUse()
    {
        // Arrange
        var transactionService = new TransactionService(_context);
        var asset = new Asset
        {
            Name = "Keyboard",
            SerialNumber = "KB-001",
            Status = AssetStatus.InStock,
            CreatedAt = DateTime.UtcNow
        };
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Act
        await transactionService.CheckOutAssetAsync(asset.Id, "TestUser");

        // Assert
        var updatedAsset = await _context.Assets.FindAsync(asset.Id);
        updatedAsset.Should().NotBeNull();
        updatedAsset!.Status.Should().Be(AssetStatus.InUse);
    }

    /// <summary>
    /// Test 5: Check-in when asset is already Available should throw exception
    /// </summary>
    [Fact]
    public async Task CheckIn_WhenAssetIsAlreadyAvailable_ThrowsException()
    {
        // Arrange
        var transactionService = new TransactionService(_context);
        var asset = new Asset
        {
            Name = "Mouse",
            SerialNumber = "MS-001",
            Status = AssetStatus.InStock,
            CreatedAt = DateTime.UtcNow
        };
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => transactionService.CheckInAssetAsync(asset.Id, "System")
        );

        exception.Message.Should().Contain("not currently checked out");
    }

    /// <summary>
    /// Test 6: Asset creation logs audit entry
    /// </summary>
    [Fact]
    public async Task CreateAsset_LogsAuditEntry()
    {
        // Arrange
        var asset = new Asset
        {
            Name = "Printer",
            SerialNumber = "PR-001"
        };

        // Act
        await _assetService.CreateAssetAsync(asset, "AdminUser");

        // Assert
        _mockAuditService.Verify(
            x => x.LogActionAsync(
                "Asset",
                "Create",
                It.Is<string>(s => s.Contains("Printer")),
                "AdminUser"
            ),
            Times.Once
        );
    }

    /// <summary>
    /// Test 7: Check-out creates transaction record with correct details
    /// </summary>
    [Fact]
    public async Task CheckOut_CreatesTransactionRecord()
    {
        // Arrange
        var transactionService = new TransactionService(_context);
        var asset = new Asset
        {
            Name = "Tablet",
            SerialNumber = "TAB-001",
            Status = AssetStatus.InStock,
            CreatedAt = DateTime.UtcNow
        };
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Act
        await transactionService.CheckOutAssetAsync(asset.Id, "JohnDoe");

        // Assert
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.AssetId == asset.Id && t.Type == TransactionType.CheckOut);
        
        transaction.Should().NotBeNull();
        transaction!.UserName.Should().Be("JohnDoe");
        transaction.AssetId.Should().Be(asset.Id);
    }

    /// <summary>
    /// Test 8: Delete asset with non-admin user throws UnauthorizedAccessException
    /// </summary>
    [Fact]
    public async Task DeleteAsset_WithNonAdminUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var asset = new Asset
        {
            Name = "Server",
            SerialNumber = "SRV-001",
            CreatedAt = DateTime.UtcNow
        };
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _assetService.DeleteAssetAsync(asset.Id, "RegularUser")
        );

        exception.Message.Should().Contain("administrator");
    }
}
