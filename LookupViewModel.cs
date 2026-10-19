using System.ComponentModel.DataAnnotations;
namespace RefugeeAppointmentSystem.Models;
public class LookupViewModel {
    [Required] public string SearchType { get; set; } = "individual";
    [Required, StringLength(120)] public string SearchValue { get; set; } = "";
    [Required, DataType(DataType.Date)] public DateTime DateOfBirth { get; set; }
    public bool Searched { get; set; }
    public string? Message { get; set; }
    public List<AppointmentResult> Results { get; set; } = new();
}
public class AppointmentResult {
    public string ServiceName { get; set; } = "";
    public DateTime AppointmentDateTime { get; set; }
    public string Location { get; set; } = "";
    public string Status { get; set; } = "";
    public string PersonDisplay { get; set; } = "";
    public string GroupNumber { get; set; } = "";
    public string IndividualNumberMasked { get; set; } = "";
}
