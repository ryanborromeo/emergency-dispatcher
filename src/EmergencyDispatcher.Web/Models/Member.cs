using System.ComponentModel.DataAnnotations;

namespace EmergencyDispatcher.Web.Models;

public class Member
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    [Required, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? EmergencyContact { get; set; }

    [MaxLength(500)]
    public string? Allergies { get; set; }

    [MaxLength(500)]
    public string? Medications { get; set; }

    [MaxLength(500)]
    public string? MedicalConditions { get; set; }

    public int? PreferredHospitalId { get; set; }
    public Hospital? PreferredHospital { get; set; }

    public bool ConsentFlag { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
