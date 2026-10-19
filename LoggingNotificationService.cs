using RefugeeAppointmentSystem.Models;
namespace RefugeeAppointmentSystem.Services;
/// <summary>Safe starter implementation. Replace with an approved SMS gateway and consent-aware delivery workflow.</summary>
public class LoggingNotificationService(ILogger<LoggingNotificationService> logger) : INotificationService {
    public Task SendAppointmentReminderAsync(Individual person, Appointment appointment, CancellationToken cancellationToken = default) {
        // Do not log full names, phone numbers, dates of birth, or registration numbers.
        logger.LogInformation("Appointment reminder queued for person record {PersonId}, appointment {AppointmentId}. Configure an approved SMS provider to deliver messages.", person.Id, appointment.Id);
        return Task.CompletedTask;
    }
}
