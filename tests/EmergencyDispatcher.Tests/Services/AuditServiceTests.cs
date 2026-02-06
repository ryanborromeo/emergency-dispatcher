using EmergencyDispatcher.Web.Services;

namespace EmergencyDispatcher.Tests.Services;

public class AuditServiceTests
{
    [Fact]
    public async Task LogActionAsync_CreatesAuditLogEntry()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new AuditService(db);

        await sut.LogActionAsync("created", "user-1", caseId: 1, details: "Test case created");

        var logs = db.AuditLogs.ToList();
        Assert.Single(logs);
        Assert.Equal("created", logs[0].Action);
        Assert.Equal("user-1", logs[0].PerformedBy);
        Assert.Equal(1, logs[0].CaseId);
        Assert.Equal("Test case created", logs[0].Details);
    }

    [Fact]
    public async Task LogActionAsync_WithoutCaseId_SavesNullCaseId()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new AuditService(db);

        await sut.LogActionAsync("login", "user-1");

        var log = db.AuditLogs.Single();
        Assert.Null(log.CaseId);
    }

    [Fact]
    public async Task LogActionAsync_SetsTimestamp()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new AuditService(db);
        var before = DateTime.UtcNow.AddSeconds(-1);

        await sut.LogActionAsync("created", "user-1");

        var log = db.AuditLogs.Single();
        Assert.True(log.Timestamp >= before);
        Assert.True(log.Timestamp <= DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task GetCaseLogsAsync_ReturnsLogsInReverseChronologicalOrder()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new AuditService(db);

        // Add user first for the Include to work
        db.Users.Add(TestHelpers.CreateTestUser());
        await db.SaveChangesAsync();

        await sut.LogActionAsync("created", "test-user-id", caseId: 1, details: "First");
        await Task.Delay(10); // Ensure different timestamps
        await sut.LogActionAsync("updated", "test-user-id", caseId: 1, details: "Second");
        await Task.Delay(10);
        await sut.LogActionAsync("notified", "test-user-id", caseId: 1, details: "Third");

        var logs = await sut.GetCaseLogsAsync(1);

        Assert.Equal(3, logs.Count);
        Assert.Equal("Third", logs[0].Details);
        Assert.Equal("Second", logs[1].Details);
        Assert.Equal("First", logs[2].Details);
    }

    [Fact]
    public async Task GetCaseLogsAsync_OnlyReturnsCaseLogs()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new AuditService(db);

        var user = TestHelpers.CreateTestUser();
        user.Id = "user-1";
        db.Users.Add(user);
        await db.SaveChangesAsync();

        await sut.LogActionAsync("created", "user-1", caseId: 1);
        await sut.LogActionAsync("created", "user-1", caseId: 2);
        await sut.LogActionAsync("updated", "user-1", caseId: 1);

        var logs = await sut.GetCaseLogsAsync(1);

        Assert.Equal(2, logs.Count);
        Assert.All(logs, l => Assert.Equal(1, l.CaseId));
    }

    [Fact]
    public async Task GetCaseLogsAsync_EmptyForNonExistentCase()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new AuditService(db);

        var logs = await sut.GetCaseLogsAsync(999);

        Assert.Empty(logs);
    }
}
