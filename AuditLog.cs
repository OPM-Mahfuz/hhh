using System.ComponentModel.DataAnnotations;
namespace RefugeeAppointmentSystem.Models;
public class AuditLog {
    public int Id { get; set; }
    [Required, StringLength(256)] public string Actor { get; set; } = "";
    [Required, StringLength(80)] public string Action { get; set; } = "";
    [StringLength(100)] public string EntityType { get; set; } = "";
    public int? EntityId { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    [StringLength(1000)] public string? Details { get; set; }
}
