using System.ComponentModel.DataAnnotations;

namespace EmergencyDispatcher.Web.Models;

public class AuditLog
{
    public int Id { get; set; }

    public int? CaseId { get; set; }
    public Case? Case { get; set; }

    [Required, MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    [Required]
    public string PerformedBy { get; set; } = string.Empty;
    public ApplicationUser? Performer { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(2000)]
    public string? Details { get; set; }
}
