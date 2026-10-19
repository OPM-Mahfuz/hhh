using System.ComponentModel.DataAnnotations;

namespace RefugeeAppointmentSystem.Models;
public class Appointment {
    public int Id { get; set; }
    [Required] public int IndividualId { get; set; }
    public Individual? Individual { get; set; }
    [Required, StringLength(100)] public string ServiceName { get; set; } = "Registration";
    [Required, DataType(DataType.DateTime)] public DateTime AppointmentDateTime { get; set; }
    [Required, StringLength(120)] public string Location { get; set; } = "";
    [Required, StringLength(20)] public string Status { get; set; } = "Scheduled"; // Scheduled, Arrived, Served, Absent, Rescheduled, Cancelled
    [StringLength(500)] public string? StaffNotes { get; set; }
    [StringLength(256)] public string CreatedBy { get; set; } = "system";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
