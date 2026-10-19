using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefugeeAppointmentSystem.Data;
using RefugeeAppointmentSystem.Models;
using RefugeeAppointmentSystem.Services;

namespace RefugeeAppointmentSystem.Controllers;
[Authorize(Roles = "Administrator,Scheduler,Viewer")]
public class StaffController(AppDbContext db, INotificationService notifications) : Controller {
    public async Task<IActionResult> Index(CancellationToken ct) {
        ViewBag.People = await db.Individuals.CountAsync(x => x.IsActive, ct);
        ViewBag.Today = await db.Appointments.CountAsync(x => x.AppointmentDateTime.Date == DateTime.Today, ct);
        ViewBag.Pending = await db.Appointments.CountAsync(x => x.Status == "Scheduled" && x.AppointmentDateTime >= DateTime.Now, ct);
        var rows = await db.Appointments.AsNoTracking().Include(x => x.Individual).OrderBy(x => x.AppointmentDateTime).Take(200).ToListAsync(ct);
        return View(rows);
    }
    [Authorize(Roles = "Administrator,Scheduler")]
    [HttpGet] public IActionResult CreateIndividual() => View(new Individual { DateOfBirth = new DateTime(2000,1,1) });
    [Authorize(Roles = "Administrator,Scheduler")]
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CreateIndividual(Individual model, CancellationToken ct) {
        if (await db.Individuals.AnyAsync(x => x.IndividualNumber == model.IndividualNumber, ct)) ModelState.AddModelError(nameof(model.IndividualNumber), "That individual number already exists.");
        if (!ModelState.IsValid) return View(model);
        db.Individuals.Add(model);
        db.AuditLogs.Add(new AuditLog { Actor = User.Identity?.Name ?? "unknown", Action = "CreateIndividual", EntityType = "Individual", Details = "Created an individual record." });
        await db.SaveChangesAsync(ct);
        TempData["Notice"] = "Individual record created.";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Administrator,Scheduler")]
    [HttpGet] public async Task<IActionResult> CreateAppointment(CancellationToken ct) {
        return View(new AppointmentCreateViewModel { AppointmentDateTime = DateTime.Now.AddDays(7), Individuals = await db.Individuals.Where(x => x.IsActive).OrderBy(x => x.FullName).ToListAsync(ct) });
    }
    [Authorize(Roles = "Administrator,Scheduler")]
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> CreateAppointment(AppointmentCreateViewModel model, CancellationToken ct) {
        model.Individuals = await db.Individuals.Where(x => x.IsActive).OrderBy(x => x.FullName).ToListAsync(ct);
        if (!await db.Individuals.AnyAsync(x => x.Id == model.IndividualId && x.IsActive, ct)) ModelState.AddModelError(nameof(model.IndividualId), "Select a valid active person.");
        if (model.AppointmentDateTime < DateTime.Now) ModelState.AddModelError(nameof(model.AppointmentDateTime), "Appointment must be in the future.");
        if (!ModelState.IsValid) return View(model);
        var appointment = new Appointment { IndividualId = model.IndividualId, ServiceName = model.ServiceName.Trim(), AppointmentDateTime = model.AppointmentDateTime, Location = model.Location.Trim(), Status = model.Status, CreatedBy = User.Identity?.Name ?? "unknown" };
        db.Appointments.Add(appointment);
        await db.SaveChangesAsync(ct);
        var person = await db.Individuals.FindAsync(new object?[] { model.IndividualId }, ct);
        if (person is not null) await notifications.SendAppointmentReminderAsync(person, appointment, ct);
        db.AuditLogs.Add(new AuditLog { Actor = User.Identity?.Name ?? "unknown", Action = "CreateAppointment", EntityType = "Appointment", EntityId = appointment.Id, Details = "Appointment created." });
        await db.SaveChangesAsync(ct);
        TempData["Notice"] = "Appointment created. SMS delivery is not enabled until a provider is configured.";
        return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles = "Administrator,Scheduler")]
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> UpdateStatus(int id, string status, CancellationToken ct) {
        var allowed = new[] { "Scheduled", "Arrived", "Served", "Absent", "Rescheduled", "Cancelled" };
        if (!allowed.Contains(status)) return BadRequest();
        var appointment = await db.Appointments.Include(x => x.Individual).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (appointment is null) return NotFound();
        appointment.Status = status; appointment.UpdatedAtUtc = DateTime.UtcNow;
        db.AuditLogs.Add(new AuditLog { Actor = User.Identity?.Name ?? "unknown", Action = "UpdateAppointmentStatus", EntityType = "Appointment", EntityId = id, Details = $"Status changed to {status}." });
        await db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }
}
