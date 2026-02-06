using EmergencyDispatcher.Web.Models;

namespace EmergencyDispatcher.Web.Services;

public interface IHospitalService
{
    Task<List<Hospital>> GetAllAsync();
    Task<Hospital?> GetByIdAsync(int id);
    Task<Hospital> CreateAsync(Hospital hospital);
    Task UpdateAsync(Hospital hospital);
}
