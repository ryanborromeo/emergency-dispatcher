using EmergencyDispatcher.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace EmergencyDispatcher.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Dispatcher;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
