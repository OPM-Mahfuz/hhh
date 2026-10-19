# Refugee Online Appointment Scheduling System

Starter implementation for a standalone appointment system using ASP.NET Core MVC, Entity Framework Core and SQL Server.

## Included
- Public appointment lookup by full name, group/household number, or individual registration number, with date-of-birth verification.
- Staff dashboard and counts for active individuals, appointments today, and upcoming appointments.
- Create individual records and appointments.
- Appointment status updates: Scheduled, Arrived, Served, Absent, Rescheduled, Cancelled.
- SQL Server models and indexes for individuals, appointments, and audit logs.
- ASP.NET Core Identity role setup: Administrator, Scheduler, Viewer.
- HTTPS redirects, HSTS outside Development, secure authentication cookie settings, antiforgery on state-changing forms.
- Notification service abstraction. The included implementation logs a queue event; it does NOT send real SMS. Connect an approved SMS gateway before enabling delivery.

## Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB for Windows development, or SQL Server/managed SQL for deployment)

## Run locally
1. Update `ConnectionStrings:DefaultConnection` in `appsettings.json` for your SQL Server.
2. From this directory run:
   ```bash
   dotnet restore
   dotnet run
   ```
3. In Development, the application calls `EnsureCreatedAsync()` to create a fresh database for a quick demonstration. Before production, replace this with reviewed EF Core migrations and a controlled deployment process.
4. Set the initial administrator credentials as environment variables before first launch:
   - `INITIAL_ADMIN_EMAIL=admin@example.org`
   - `INITIAL_ADMIN_PASSWORD` set to a long, unique password (12+ characters; upper/lowercase and digit)
   The initial account is assigned the Administrator role. Do not put production passwords in source control or appsettings files.
5. Browse to the HTTPS URL shown by `dotnet run`. Public lookup is on `/`; staff dashboard is `/Staff`.

## SQL Server configuration
Example connection string for a server deployment (use secrets/environment configuration):
`Server=YOUR_SQL_SERVER;Database=RefugeeAppointments;Encrypt=True;TrustServerCertificate=False;User ID=...;Password=...`

## Important production work still required
This is a runnable starter codebase, not an approved production deployment. Before using real refugee data:
- Complete a privacy/security assessment and obtain OPM/UNHCR data-governance approval.
- Use a verified identity lookup flow. The prototype requires DOB and returns only the matched person's appointment; assess whether DOB alone is sufficient and add an approved one-time code or assisted lookup where appropriate.
- Replace development database creation with EF Core migrations, backups, retention policies, monitoring, and a tested recovery plan.
- Configure a real SMS provider, sender ID, delivery receipts, retry rules, consent/legal basis, message templates, and opt-out/help workflow. Do not include sensitive protection details in SMS.
- Configure HTTPS certificates, secret storage, rate limiting, anti-automation controls, lockout/monitoring, and production error handling.
- Add staff account lifecycle management, MFA, settlement/duty-station scoping, audit-log review, data export controls, and authorization tests.
- Add duplicate/overlapping appointment checks, slot capacity management, group bulk scheduling, rescheduling workflows, and report export before operational rollout.
- Use synthetic test data only during development. Never deploy the example using real refugee records without formal authorization.

## Main data entities
- `Individual`: unique individual number, group number, name, date of birth, optional phone, settlement, active flag.
- `Appointment`: person, service, date/time, location, status, creator, timestamps.
- `AuditLog`: actor, action, entity type/ID, timestamp, minimal details.
- ASP.NET Identity tables: users, roles, claims, logins, tokens.

## API / integration direction
The initial version is standalone. If integration with ProGres V4 is approved later, implement a separate authenticated integration service or import workflow. Do not directly modify ProGres records from this application without an authorized interface and governance approval.
