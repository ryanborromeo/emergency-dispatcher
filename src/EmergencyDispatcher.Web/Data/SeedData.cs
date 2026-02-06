using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace EmergencyDispatcher.Web.Data;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create roles
        string[] roles = ["Admin", "Dispatcher"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        if (await userManager.FindByEmailAsync("admin@dispatch.com") is null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@dispatch.com",
                Email = "admin@dispatch.com",
                FullName = "System Administrator",
                PhoneNumber = "+1-555-0100",
                Role = UserRole.Admin,
                IsActive = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(admin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // Create dispatcher user
        if (await userManager.FindByEmailAsync("dispatcher@dispatch.com") is null)
        {
            var dispatcher = new ApplicationUser
            {
                UserName = "dispatcher@dispatch.com",
                Email = "dispatcher@dispatch.com",
                FullName = "Maria Santos",
                PhoneNumber = "+1-555-0101",
                Role = UserRole.Dispatcher,
                IsActive = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(dispatcher, "Dispatch123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(dispatcher, "Dispatcher");
            }
        }

        // Seed hospitals
        if (!db.Hospitals.Any())
        {
            db.Hospitals.AddRange(
                new Hospital
                {
                    Name = "Metro General Hospital",
                    TriageContactName = "Dr. Elena Cruz",
                    TriagePhone = "+1-555-0201",
                    TriageWhatsApp = "+1-555-0201",
                    PreferredNotificationMethod = NotificationMethod.Call
                },
                new Hospital
                {
                    Name = "St. Mary's Medical Center",
                    TriageContactName = "Nurse James Rivera",
                    TriagePhone = "+1-555-0202",
                    TriageWhatsApp = "+1-555-0202",
                    PreferredNotificationMethod = NotificationMethod.WhatsApp
                },
                new Hospital
                {
                    Name = "City Medical Center",
                    TriageContactName = "Dr. Amir Patel",
                    TriagePhone = "+1-555-0203",
                    TriageWhatsApp = "+1-555-0203",
                    PreferredNotificationMethod = NotificationMethod.Call
                }
            );
            await db.SaveChangesAsync();
        }

        // Seed members
        if (!db.Members.Any())
        {
            var metroHospital = db.Hospitals.First(h => h.Name == "Metro General Hospital");

            db.Members.AddRange(
                new Member
                {
                    FullName = "Juan Dela Cruz",
                    DateOfBirth = new DateOnly(1985, 3, 15),
                    Phone = "+1-555-1001",
                    EmergencyContact = "Ana Dela Cruz (+1-555-1002)",
                    Allergies = "Penicillin",
                    Medications = "Metformin 500mg, Lisinopril 10mg",
                    MedicalConditions = "Type 2 Diabetes, Hypertension",
                    PreferredHospitalId = metroHospital.Id,
                    ConsentFlag = true
                },
                new Member
                {
                    FullName = "Rosa Martinez",
                    DateOfBirth = new DateOnly(1972, 8, 22),
                    Phone = "+1-555-1003",
                    EmergencyContact = "Carlos Martinez (+1-555-1004)",
                    Allergies = "Sulfa drugs, Shellfish",
                    Medications = "Amlodipine 5mg",
                    MedicalConditions = "Hypertension, Asthma",
                    PreferredHospitalId = metroHospital.Id,
                    ConsentFlag = true
                },
                new Member
                {
                    FullName = "Miguel Santos",
                    DateOfBirth = new DateOnly(1990, 11, 5),
                    Phone = "+1-555-1005",
                    EmergencyContact = "Lisa Santos (+1-555-1006)",
                    Allergies = null,
                    Medications = null,
                    MedicalConditions = "Epilepsy",
                    ConsentFlag = true
                }
            );
            await db.SaveChangesAsync();
        }
    }
}
