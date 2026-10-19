using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RefugeeAppointmentSystem.Models;
namespace RefugeeAppointmentSystem.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext(options) {
    public DbSet<Individual> Individuals => Set<Individual>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
        builder.Entity<Individual>().HasIndex(x => x.IndividualNumber).IsUnique();
        builder.Entity<Individual>().HasIndex(x => x.GroupNumber);
        builder.Entity<Individual>().HasIndex(x => x.FullName);
        builder.Entity<Appointment>().HasIndex(x => new { x.AppointmentDateTime, x.Status });
        builder.Entity<Appointment>().HasOne(x => x.Individual).WithMany(x => x.Appointments).HasForeignKey(x => x.IndividualId).OnDelete(DeleteBehavior.Restrict);
    }
}
