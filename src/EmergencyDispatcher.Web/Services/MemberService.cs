using EmergencyDispatcher.Web.Data;
using EmergencyDispatcher.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EmergencyDispatcher.Web.Services;

public class MemberService : IMemberService
{
    private readonly ApplicationDbContext _db;

    public MemberService(ApplicationDbContext db) => _db = db;

    public async Task<List<Member>> SearchAsync(string? query = null)
    {
        var q = _db.Members
            .Include(m => m.PreferredHospital)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var lower = query.ToLower();
            q = q.Where(m =>
                m.FullName.ToLower().Contains(lower) ||
                m.Phone.Contains(query));
        }

        return await q
            .OrderBy(m => m.FullName)
            .ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        return await _db.Members
            .Include(m => m.PreferredHospital)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Member?> LookupByPhoneAsync(string phone)
    {
        return await _db.Members
            .Include(m => m.PreferredHospital)
            .FirstOrDefaultAsync(m => m.Phone == phone);
    }

    public async Task<Member> CreateAsync(Member member)
    {
        member.CreatedAt = DateTime.UtcNow;
        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(Member member)
    {
        _db.Members.Update(member);
        await _db.SaveChangesAsync();
    }
}
