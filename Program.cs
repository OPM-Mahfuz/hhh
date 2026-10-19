using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RefugeeAppointmentSystem.Data;
using RefugeeAppointmentSystem.Models;
using RefugeeAppointmentSystem.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.ConfigureApplicationCookie(options => { options.Cookie.HttpOnly = true; options.Cookie.SecurePolicy = CookieSecurePolicy.Always; options.Cookie.SameSite = SameSiteMode.Lax; options.LoginPath = "/Identity/Account/Login"; });
builder.Services.AddScoped<INotificationService, LoggingNotificationService>();
builder.Services.AddAntiforgery(options => options.Cookie.SecurePolicy = CookieSecurePolicy.Always);
builder.Services.AddHsts(options => { options.MaxAge = TimeSpan.FromDays(180); options.IncludeSubDomains = true; });

var app = builder.Build();
if (app.Environment.IsDevelopment()) { using var dbScope = app.Services.CreateScope(); var db = dbScope.ServiceProvider.GetRequiredService<AppDbContext>(); await db.Database.EnsureCreatedAsync(); }
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection(); app.UseStaticFiles(); app.UseRouting(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
await SeedRolesAsync(app.Services);
app.Run();

static async Task SeedRolesAsync(IServiceProvider services) {
    using var scope = services.CreateScope();
    var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Administrator", "Scheduler", "Viewer" })
        if (!await roles.RoleExistsAsync(role)) await roles.CreateAsync(new IdentityRole(role));
    // Create an initial admin only when explicitly configured through environment variables.
    var email = Environment.GetEnvironmentVariable("INITIAL_ADMIN_EMAIL");
    var password = Environment.GetEnvironmentVariable("INITIAL_ADMIN_PASSWORD");
    if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password)) {
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await users.FindByEmailAsync(email);
        if (user is null) {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await users.CreateAsync(user, password);
            if (result.Succeeded) await users.AddToRoleAsync(user, "Administrator");
        }
    }
}
