using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Services;

namespace EmergencyDispatcher.Tests.Services;

public class MemberServiceTests
{
    [Fact]
    public async Task LookupByPhoneAsync_ReturnsCorrectMember()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        var member = TestHelpers.CreateTestMember();
        member.Id = 0;
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var result = await sut.LookupByPhoneAsync("+1-555-1001");

        Assert.NotNull(result);
        Assert.Equal("Juan Dela Cruz", result.FullName);
    }

    [Fact]
    public async Task LookupByPhoneAsync_ReturnsNullForUnknownPhone()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);

        var result = await sut.LookupByPhoneAsync("+1-555-9999");

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_FiltersByName()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        db.Members.AddRange(
            new Member { FullName = "Juan Dela Cruz", Phone = "+1-555-0001" },
            new Member { FullName = "Rosa Martinez", Phone = "+1-555-0002" },
            new Member { FullName = "Miguel Santos", Phone = "+1-555-0003" }
        );
        await db.SaveChangesAsync();

        var result = await sut.SearchAsync("Juan");

        Assert.Single(result);
        Assert.Equal("Juan Dela Cruz", result[0].FullName);
    }

    [Fact]
    public async Task SearchAsync_FiltersByPhone()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        db.Members.AddRange(
            new Member { FullName = "Juan Dela Cruz", Phone = "+1-555-0001" },
            new Member { FullName = "Rosa Martinez", Phone = "+1-555-0002" }
        );
        await db.SaveChangesAsync();

        var result = await sut.SearchAsync("0002");

        Assert.Single(result);
        Assert.Equal("Rosa Martinez", result[0].FullName);
    }

    [Fact]
    public async Task SearchAsync_ReturnsAllWhenNoQuery()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        db.Members.AddRange(
            new Member { FullName = "Juan Dela Cruz", Phone = "+1-555-0001" },
            new Member { FullName = "Rosa Martinez", Phone = "+1-555-0002" }
        );
        await db.SaveChangesAsync();

        var result = await sut.SearchAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task SearchAsync_IsCaseInsensitive()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        db.Members.Add(new Member { FullName = "Juan Dela Cruz", Phone = "+1-555-0001" });
        await db.SaveChangesAsync();

        var result = await sut.SearchAsync("juan");

        Assert.Single(result);
    }

    [Fact]
    public async Task CreateAsync_SetsCreatedAtAndSaves()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        var member = new Member
        {
            FullName = "New Member",
            Phone = "+1-555-9000"
        };

        var before = DateTime.UtcNow;
        var result = await sut.CreateAsync(member);

        Assert.True(result.Id > 0);
        Assert.True(result.CreatedAt >= before);
        Assert.Single(db.Members);
    }

    [Fact]
    public async Task UpdateAsync_SavesChanges()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        var member = new Member { FullName = "Original", Phone = "+1-555-0001" };
        db.Members.Add(member);
        await db.SaveChangesAsync();

        member.FullName = "Updated Name";
        await sut.UpdateAsync(member);

        var updated = await db.Members.FindAsync(member.Id);
        Assert.Equal("Updated Name", updated!.FullName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForNonExistent()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);

        var result = await sut.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMemberWithHospital()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var sut = new MemberService(db);
        var hospital = TestHelpers.CreateTestHospital();
        hospital.Id = 0;
        db.Hospitals.Add(hospital);
        await db.SaveChangesAsync();

        var member = TestHelpers.CreateTestMember();
        member.Id = 0;
        member.PreferredHospitalId = hospital.Id;
        db.Members.Add(member);
        await db.SaveChangesAsync();

        var result = await sut.GetByIdAsync(member.Id);

        Assert.NotNull(result);
        Assert.NotNull(result.PreferredHospital);
        Assert.Equal("Metro General Hospital", result.PreferredHospital.Name);
    }
}
