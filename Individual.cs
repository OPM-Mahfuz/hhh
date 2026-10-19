using System.ComponentModel.DataAnnotations;

namespace RefugeeAppointmentSystem.Models;
public class Individual {
    public int Id { get; set; }
    [Required, StringLength(120)] public string FullName { get; set; } = "";
    [Required, StringLength(40)] public string IndividualNumber { get; set; } = "";
    [Required, StringLength(40)] public string GroupNumber { get; set; } = "";
    [Required, DataType(DataType.Date)] public DateTime DateOfBirth { get; set; }
    [StringLength(30)] public string? PhoneNumber { get; set; }
    [StringLength(100)] public string Settlement { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public List<Appointment> Appointments { get; set; } = new();
}
