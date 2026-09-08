using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventLensAI.Infrastructure.Persistence;

/// <summary>Creates the local development administrator. Never enable this seed in production.</summary>
public sealed class DevelopmentSuperAdminSeeder(
    EventLensDbContext db,
    IPasswordService passwords,
    IConfiguration configuration,
    ILogger<DevelopmentSuperAdminSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var section = configuration.GetSection("DevelopmentSeed:SuperAdmin");
        if (!section.GetValue<bool>("Enabled")) return;

        var email = section["Email"]?.Trim().ToLowerInvariant()
            ?? throw new InvalidOperationException("DevelopmentSeed:SuperAdmin:Email is required.");
        var password = section["Password"]
            ?? throw new InvalidOperationException("DevelopmentSeed:SuperAdmin:Password is required.");
        if (password.Length < 12)
            throw new InvalidOperationException("The development Super Admin password must contain at least 12 characters.");

        await db.Database.MigrateAsync(cancellationToken);
        var user = await db.Users
            .Include(item => item.Memberships)
            .SingleOrDefaultAsync(item => item.NormalizedEmail == email.ToUpperInvariant(), cancellationToken);

        if (user is null)
        {
            user = new User("Super", "Admin", email, passwords.Hash(password), null);
            user.VerifyEmail();
            db.Users.Add(user);
        }

        var systemOrganization = await db.Organizations
            .SingleOrDefaultAsync(item => item.Slug == "eventlens-system", cancellationToken);
        if (systemOrganization is null)
        {
            systemOrganization = new Organization("EventLens System", "eventlens-system", user.Id);
            db.Organizations.Add(systemOrganization);
        }

        if (!await db.OrganizationMembers.AnyAsync(
            item => item.UserId == user.Id && item.RoleId == SystemRoles.SuperAdminId,
            cancellationToken))
        {
            db.OrganizationMembers.Add(new OrganizationMember(
                systemOrganization.Id, user.Id, SystemRoles.SuperAdminId));
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Development Super Admin is ready: {Email}", email);
    }
}
