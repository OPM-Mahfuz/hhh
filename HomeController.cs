using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RefugeeAppointmentSystem.Data;
using RefugeeAppointmentSystem.Models;

namespace RefugeeAppointmentSystem.Controllers;
public class HomeController(AppDbContext db) : Controller {
    [HttpGet] public IActionResult Index() => View(new LookupViewModel());
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Lookup(LookupViewModel model, CancellationToken ct) {
        model.Searched = true;
        if (!ModelState.IsValid) { model.Message = "Enter the requested search details."; return View("Index", model); }
        var value = model.SearchValue.Trim();
        var dob = model.DateOfBirth.Date;
        Individual? person = model.SearchType.ToLowerInvariant() switch {
            "name" => await db.Individuals.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive && x.FullName == value && x.DateOfBirth.Date == dob, ct),
            "group" => await db.Individuals.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive && x.GroupNumber == value && x.DateOfBirth.Date == dob, ct),
            _ => await db.Individuals.AsNoTracking().FirstOrDefaultAsync(x => x.IsActive && x.IndividualNumber == value && x.DateOfBirth.Date == dob, ct)
        };
        if (person is null) {
            model.Message = "No matching record was found. Check the details or contact your registration centre for assistance.";
            return View("Index", model);
        }
        var now = DateTime.Now;
        model.Results = await db.Appointments.AsNoTracking()
            .Where(a => a.IndividualId == person.Id && a.Status != "Cancelled" && a.AppointmentDateTime >= now.AddDays(-30))
            .OrderBy(a => a.AppointmentDateTime)
            .Select(a => new AppointmentResult {
                ServiceName = a.ServiceName, AppointmentDateTime = a.AppointmentDateTime,
                Location = a.Location, Status = a.Status,
                PersonDisplay = person.FullName, GroupNumber = person.GroupNumber,
                IndividualNumberMasked = person.IndividualNumber.Length > 4 ? "••••" + person.IndividualNumber.Substring(person.IndividualNumber.Length - 4) : "••••"
            }).ToListAsync(ct);
        model.Message = model.Results.Count == 0 ? "Your identity was verified, but no current appointment was found. Please contact your registration centre." : null;
        return View("Index", model);
    }
    [HttpGet] public IActionResult Privacy() => View();
    [HttpGet] public IActionResult Error() => View();
}
