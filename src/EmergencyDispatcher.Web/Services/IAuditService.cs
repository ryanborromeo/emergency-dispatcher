using EmergencyDispatcher.Web.Models;

namespace EmergencyDispatcher.Web.Services;

public interface IAuditService
{
    Task LogActionAsync(string action, string userId, int? caseId = null, string? details = null);
    Task<List<AuditLog>> GetCaseLogsAsync(int caseId);
}
