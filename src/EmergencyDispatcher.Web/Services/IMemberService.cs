using EmergencyDispatcher.Web.Models;

namespace EmergencyDispatcher.Web.Services;

public interface IMemberService
{
    Task<List<Member>> SearchAsync(string? query = null);
    Task<Member?> GetByIdAsync(int id);
    Task<Member?> LookupByPhoneAsync(string phone);
    Task<Member> CreateAsync(Member member);
    Task UpdateAsync(Member member);
}
