using EmergencyDispatcher.Web.Data;
using EmergencyDispatcher.Web.Models;
using EmergencyDispatcher.Web.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace EmergencyDispatcher.Web.Services;

public class CaseService : ICaseService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public CaseService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<List<Case>> GetActiveCasesAsync(CaseStatus? filterStatus = null)
    {
        var query = _db.Cases
            .Include(c => c.Hospital)
            .Include(c => c.Member)
            .Include(c => c.Creator)
            .AsQueryable();

        if (filterStatus.HasValue)
        {
            query = query.Where(c => c.Status == filterStatus.Value);
        }
        else
        {
            query = query.Where(c => c.Status != CaseStatus.Closed);
        }

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Case?> GetByIdAsync(int id)
    {
        return await _db.Cases
            .Include(c => c.Hospital)
            .Include(c => c.Member)
            .Include(c => c.Creator)
            .Include(c => c.AuditLogs.OrderByDescending(a => a.Timestamp))
                .ThenInclude(a => a.Performer)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Case> CreateAsync(Case newCase, string userId)
    {
        newCase.CreatedBy = userId;
        newCase.CreatedAt = DateTime.UtcNow;
        newCase.Status = CaseStatus.Open;

        _db.Cases.Add(newCase);
        await _db.SaveChangesAsync();

        await _audit.LogActionAsync("created", userId, newCase.Id,
            $"Case created for patient {newCase.PatientName}");

        return newCase;
    }

    public async Task UpdateAsync(Case updatedCase, string userId)
    {
        _db.Cases.Update(updatedCase);
        await _db.SaveChangesAsync();

        await _audit.LogActionAsync("updated", userId, updatedCase.Id,
            "Case details updated");
    }

    public async Task UpdateStatusAsync(int id, CaseStatus status, string userId)
    {
        var existingCase = await _db.Cases.FindAsync(id)
            ?? throw new InvalidOperationException($"Case {id} not found");

        var oldStatus = existingCase.Status;
        existingCase.Status = status;
        await _db.SaveChangesAsync();

        await _audit.LogActionAsync("status_changed", userId, id,
            $"Status changed from {oldStatus} to {status}");
    }

    public async Task LogNotificationAsync(int id, NotificationMethod method, string? notes, string userId)
    {
        var existingCase = await _db.Cases.FindAsync(id)
            ?? throw new InvalidOperationException($"Case {id} not found");

        existingCase.NotifiedAt = DateTime.UtcNow;
        existingCase.NotifiedVia = method;

        if (existingCase.Status == CaseStatus.Open)
        {
            existingCase.Status = CaseStatus.Notified;
        }

        await _db.SaveChangesAsync();

        await _audit.LogActionAsync("notified", userId, id,
            $"Hospital notified via {method}" + (notes != null ? $": {notes}" : ""));
    }
}
