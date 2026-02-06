using System.ComponentModel.DataAnnotations;
using EmergencyDispatcher.Web.Models.Enums;

namespace EmergencyDispatcher.Web.Models;

public class Case
{
    public int Id { get; set; }

    public int? MemberId { get; set; }
    public Member? Member { get; set; }

    [Required, MaxLength(200)]
    public string PatientName { get; set; } = string.Empty;

    public int? Age { get; set; }

    [MaxLength(20)]
    public string? Sex { get; set; }

    [Required, MaxLength(200)]
    public string EmergencyType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? LocationText { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [MaxLength(100)]
    public string? TransportMethod { get; set; }

    public int? EstimatedEta { get; set; }

    public CaseStatus Status { get; set; } = CaseStatus.Open;

    public int? HospitalId { get; set; }
    public Hospital? Hospital { get; set; }

    public DateTime? NotifiedAt { get; set; }
    public NotificationMethod? NotifiedVia { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string CreatedBy { get; set; } = string.Empty;
    public ApplicationUser? Creator { get; set; }

    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
