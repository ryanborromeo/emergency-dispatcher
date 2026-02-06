using EmergencyDispatcher.Web.Models;

namespace EmergencyDispatcher.Web.Services;

public interface ISbarGenerator
{
    string GenerateCallScript(Case emergencyCase, Member? member, ApplicationUser dispatcher);
    string GenerateMessage(Case emergencyCase, Member? member, ApplicationUser dispatcher);
    string GenerateEmail(Case emergencyCase, Member? member, ApplicationUser dispatcher);
}
