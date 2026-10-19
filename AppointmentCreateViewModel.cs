using System.ComponentModel.DataAnnotations;
namespace RefugeeAppointmentSystem.Models;
public class AppointmentCreateViewModel {
    [Required] public int IndividualId { get; set; }
    [Required, StringLength(100)] public string ServiceName { get; set; } = "Registration";
    [Required] public DateTime AppointmentDateTime { get; set; }
    [Required, StringLength(120)] public string Location { get; set; } = "";
    [Required] public string Status { get; set; } = "Scheduled";
    public List<Individual> Individuals { get; set; } = new();
}
