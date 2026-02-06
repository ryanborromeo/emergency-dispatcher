using EmergencyDispatcher.Web.Data;
using EmergencyDispatcher.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EmergencyDispatcher.Web.Services;

public class HospitalService : IHospitalService
{
    private readonly ApplicationDbContext _db;

    public HospitalService(ApplicationDbContext db) => _db = db;

    public async Task<List<Hospital>> GetAllAsync()
    {
        return await _db.Hospitals
            .OrderBy(h => h.Name)
            .ToListAsync();
    }

    public async Task<Hospital?> GetByIdAsync(int id)
    {
        return await _db.Hospitals.FindAsync(id);
    }

    public async Task<Hospital> CreateAsync(Hospital hospital)
    {
        _db.Hospitals.Add(hospital);
        await _db.SaveChangesAsync();
        return hospital;
    }

    public async Task UpdateAsync(Hospital hospital)
    {
        _db.Hospitals.Update(hospital);
        await _db.SaveChangesAsync();
    }
}
