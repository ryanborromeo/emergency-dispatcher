using EmergencyDispatcher.Web.Data;
using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EmergencyDispatcher.Tests;

public static class TestHelpers
{
    public static ApplicationDbContext CreateInMemoryDb(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static IConfiguration CreateMockConfig(string serviceName = "Test Dispatch Service")
    {
        var inMemory = new Dictionary<string, string?>
        {
            ["ServiceName"] = serviceName
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemory)
            .Build();
    }

    public static Case CreateTestCase(int id = 1) => new()
    {
        Id = id,
        PatientName = "Juan Dela Cruz",
        Age = 38,
        Sex = "Male",
        EmergencyType = "Chest Pain",
        LocationText = "123 Main Street, Barangay Centro",
        TransportMethod = "Ambulance",
        EstimatedEta = 15,
        Status = CaseStatus.Open,
        Notes = "Severe chest pain, shortness of breath",
        CreatedBy = "test-user-id",
        CreatedAt = DateTime.UtcNow
    };

    public static Member CreateTestMember(int id = 1) => new()
    {
        Id = id,
        FullName = "Juan Dela Cruz",
        DateOfBirth = new DateOnly(1985, 3, 15),
        Phone = "+1-555-1001",
        EmergencyContact = "Ana Dela Cruz (+1-555-1002)",
        Allergies = "Penicillin",
        Medications = "Metformin 500mg, Lisinopril 10mg",
        MedicalConditions = "Type 2 Diabetes, Hypertension",
        ConsentFlag = true,
        CreatedAt = DateTime.UtcNow
    };

    public static ApplicationUser CreateTestUser() => new()
    {
        Id = "test-user-id",
        UserName = "dispatcher@test.com",
        Email = "dispatcher@test.com",
        FullName = "Test Dispatcher",
        PhoneNumber = "+1-555-0100",
        Role = UserRole.Dispatcher,
        IsActive = true
    };

    public static Hospital CreateTestHospital(int id = 1) => new()
    {
        Id = id,
        Name = "Metro General Hospital",
        TriageContactName = "Dr. Elena Cruz",
        TriagePhone = "+1-555-0201",
        TriageWhatsApp = "+1-555-0201",
        PreferredNotificationMethod = NotificationMethod.Call
    };
}
