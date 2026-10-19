using RefugeeAppointmentSystem.Models;
namespace RefugeeAppointmentSystem.Services;
public interface INotificationService { Task SendAppointmentReminderAsync(Individual person, Appointment appointment, CancellationToken cancellationToken = default); }
