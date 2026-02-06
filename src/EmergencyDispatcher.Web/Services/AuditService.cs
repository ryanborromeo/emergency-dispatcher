using EmergencyDispatcher.Web.Data;
using EmergencyDispatcher.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EmergencyDispatcher.Web.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;

    public AuditService(ApplicationDbContext db) => _db = db;

    public async Task LogActionAsync(string action, string userId, int? caseId = null, string? details = null)
    {
        var log = new AuditLog
        {
            CaseId = caseId,
            Action = action,
            PerformedBy = userId,
            Details = details
        };
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetCaseLogsAsync(int caseId)
    {
        return await _db.AuditLogs
            .Where(l => l.CaseId == caseId)
            .OrderByDescending(l => l.Timestamp)
            .Include(l => l.Performer)
            .ToListAsync();
    }
}
