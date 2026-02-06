using System.ComponentModel.DataAnnotations;
using EmergencyDispatcher.Web.Models.Enums;

namespace EmergencyDispatcher.Web.Models;

public class Hospital
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? TriageContactName { get; set; }

    [MaxLength(20)]
    public string? TriagePhone { get; set; }

    [MaxLength(20)]
    public string? TriageWhatsApp { get; set; }

    public NotificationMethod PreferredNotificationMethod { get; set; } = NotificationMethod.Call;
}
