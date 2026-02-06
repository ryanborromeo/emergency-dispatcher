using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Models.Enums;

namespace EmergencyDispatcher.Web.Services;

public interface ICaseService
{
    Task<List<Case>> GetActiveCasesAsync(CaseStatus? filterStatus = null);
    Task<Case?> GetByIdAsync(int id);
    Task<Case> CreateAsync(Case newCase, string userId);
    Task UpdateAsync(Case updatedCase, string userId);
    Task UpdateStatusAsync(int id, CaseStatus status, string userId);
    Task LogNotificationAsync(int id, NotificationMethod method, string? notes, string userId);
}
