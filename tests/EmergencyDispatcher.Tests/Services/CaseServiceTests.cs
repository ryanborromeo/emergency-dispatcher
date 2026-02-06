using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Models.Enums;
using EmergencyDispatcher.Web.Services;
using Moq;

namespace EmergencyDispatcher.Tests.Services;

public class CaseServiceTests
{
    private readonly Mock<IAuditService> _auditMock = new();

    [Fact]
    public async Task CreateAsync_SavesCaseAndLogsAudit()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        var newCase = TestHelpers.CreateTestCase();
        newCase.Id = 0; // Let EF assign

        var result = await sut.CreateAsync(newCase, "user-1");

        Assert.True(result.Id > 0);
        Assert.Equal("user-1", result.CreatedBy);
        Assert.Equal(CaseStatus.Open, result.Status);
        Assert.Single(db.Cases);

        _auditMock.Verify(a => a.LogActionAsync(
            "created", "user-1", It.IsAny<int>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ChangesStatusAndLogsAudit()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        var testCase = TestHelpers.CreateTestCase();
        testCase.Id = 0;
        db.Cases.Add(testCase);
        await db.SaveChangesAsync();

        await sut.UpdateStatusAsync(testCase.Id, CaseStatus.Notified, "user-1");

        var updated = await db.Cases.FindAsync(testCase.Id);
        Assert.Equal(CaseStatus.Notified, updated!.Status);

        _auditMock.Verify(a => a.LogActionAsync(
            "status_changed", "user-1", testCase.Id, It.Is<string>(s => s.Contains("Notified"))), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_NonExistentCase_Throws()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.UpdateStatusAsync(999, CaseStatus.Closed, "user-1"));
    }

    private static async Task SeedTestUser(EmergencyDispatcher.Web.Data.ApplicationDbContext db, string userId = "u")
    {
        if (!db.Users.Any(u => u.Id == userId))
        {
            db.Users.Add(new ApplicationUser { Id = userId, UserName = $"{userId}@test.com", FullName = "Test User" });
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetActiveCasesAsync_ExcludesClosedByDefault()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        await SeedTestUser(db);

        db.Cases.AddRange(
            new Case { PatientName = "Open", EmergencyType = "Test", Status = CaseStatus.Open, CreatedBy = "u" },
            new Case { PatientName = "Notified", EmergencyType = "Test", Status = CaseStatus.Notified, CreatedBy = "u" },
            new Case { PatientName = "Closed", EmergencyType = "Test", Status = CaseStatus.Closed, CreatedBy = "u" }
        );
        await db.SaveChangesAsync();

        var result = await sut.GetActiveCasesAsync();

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, c => c.Status == CaseStatus.Closed);
    }

    [Fact]
    public async Task GetActiveCasesAsync_FiltersByStatus()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        await SeedTestUser(db);

        db.Cases.AddRange(
            new Case { PatientName = "Open1", EmergencyType = "Test", Status = CaseStatus.Open, CreatedBy = "u" },
            new Case { PatientName = "Open2", EmergencyType = "Test", Status = CaseStatus.Open, CreatedBy = "u" },
            new Case { PatientName = "Notified", EmergencyType = "Test", Status = CaseStatus.Notified, CreatedBy = "u" }
        );
        await db.SaveChangesAsync();

        var result = await sut.GetActiveCasesAsync(CaseStatus.Open);

        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(CaseStatus.Open, c.Status));
    }

    [Fact]
    public async Task GetActiveCasesAsync_ReturnsOrderedByCreatedAtDescending()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        await SeedTestUser(db);

        db.Cases.AddRange(
            new Case { PatientName = "First", EmergencyType = "Test", Status = CaseStatus.Open, CreatedBy = "u", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new Case { PatientName = "Second", EmergencyType = "Test", Status = CaseStatus.Open, CreatedBy = "u", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new Case { PatientName = "Third", EmergencyType = "Test", Status = CaseStatus.Open, CreatedBy = "u", CreatedAt = DateTime.UtcNow }
        );
        await db.SaveChangesAsync();

        var result = await sut.GetActiveCasesAsync();

        Assert.Equal("Third", result[0].PatientName);
        Assert.Equal("Second", result[1].PatientName);
        Assert.Equal("First", result[2].PatientName);
    }

    [Fact]
    public async Task LogNotificationAsync_UpdatesNotifiedAtAndNotifiedVia()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        var testCase = TestHelpers.CreateTestCase();
        testCase.Id = 0;
        db.Cases.Add(testCase);
        await db.SaveChangesAsync();

        var before = DateTime.UtcNow;
        await sut.LogNotificationAsync(testCase.Id, NotificationMethod.Call, "Spoke to triage nurse", "user-1");

        var updated = await db.Cases.FindAsync(testCase.Id);
        Assert.NotNull(updated!.NotifiedAt);
        Assert.True(updated.NotifiedAt >= before);
        Assert.Equal(NotificationMethod.Call, updated.NotifiedVia);
        Assert.Equal(CaseStatus.Notified, updated.Status);

        _auditMock.Verify(a => a.LogActionAsync(
            "notified", "user-1", testCase.Id, It.Is<string>(s => s.Contains("Call"))), Times.Once);
    }

    [Fact]
    public async Task LogNotificationAsync_DoesNotRevertStatusIfAlreadyAdvanced()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        var testCase = TestHelpers.CreateTestCase();
        testCase.Id = 0;
        testCase.Status = CaseStatus.EnRoute;
        db.Cases.Add(testCase);
        await db.SaveChangesAsync();

        await sut.LogNotificationAsync(testCase.Id, NotificationMethod.WhatsApp, null, "user-1");

        var updated = await db.Cases.FindAsync(testCase.Id);
        Assert.Equal(CaseStatus.EnRoute, updated!.Status); // Should not revert to Notified
    }

    [Fact]
    public async Task UpdateAsync_SavesChangesAndLogsAudit()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);
        var testCase = TestHelpers.CreateTestCase();
        testCase.Id = 0;
        db.Cases.Add(testCase);
        await db.SaveChangesAsync();

        testCase.Notes = "Updated notes";
        await sut.UpdateAsync(testCase, "user-1");

        var updated = await db.Cases.FindAsync(testCase.Id);
        Assert.Equal("Updated notes", updated!.Notes);

        _auditMock.Verify(a => a.LogActionAsync(
            "updated", "user-1", testCase.Id, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForNonExistent()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new CaseService(db, _auditMock.Object);

        var result = await sut.GetByIdAsync(999);

        Assert.Null(result);
    }
}
