using EventLensAI.Application.Interfaces.Identity;
using EventLensAI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EventLensAI.Infrastructure.Persistence;

/// <summary>Creates local users for role-based UI testing. Never enable this seed in production.</summary>
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

        var superAdminEmail = section["Email"]?.Trim().ToLowerInvariant()
            ?? throw new InvalidOperationException("DevelopmentSeed:SuperAdmin:Email is required.");
        var password = section["Password"]
            ?? throw new InvalidOperationException("DevelopmentSeed:SuperAdmin:Password is required.");
        if (password.Length < 12)
            throw new InvalidOperationException("The development Super Admin password must contain at least 12 characters.");

        await db.Database.MigrateAsync(cancellationToken);
        var accounts = new[]
        {
            new SeedAccount("Super", "Admin", superAdminEmail, SystemRoles.SuperAdminId),
            new SeedAccount("Platform", "Admin", "platformadmin@eventlens.local", SystemRoles.PlatformAdminId),
            new SeedAccount("Organization", "Owner", "owner@eventlens.local", SystemRoles.OwnerId),
            new SeedAccount("Event", "Manager", "manager@eventlens.local", SystemRoles.ManagerId),
            new SeedAccount("Event", "Photographer", "photographer@eventlens.local", SystemRoles.PhotographerId),
            new SeedAccount("Booth", "Operator", "boothoperator@eventlens.local", SystemRoles.BoothOperatorId),
            new SeedAccount("Event", "Designer", "designer@eventlens.local", SystemRoles.DesignerId),
            new SeedAccount("Marketing", "Manager", "marketingmanager@eventlens.local", SystemRoles.MarketingManagerId),
            new SeedAccount("Event", "Guest", "guest@eventlens.local", SystemRoles.GuestId),
            new SeedAccount("Event", "Viewer", "viewer@eventlens.local", SystemRoles.ViewerId),
        };

        var superAdmin = await FindOrCreateUserAsync(accounts[0], password, cancellationToken);

        var systemOrganization = await db.Organizations
            .SingleOrDefaultAsync(item => item.Slug == "eventlens-system", cancellationToken);
        if (systemOrganization is null)
        {
            systemOrganization = new Organization("EventLens System", "eventlens-system", superAdmin.Id);
            db.Organizations.Add(systemOrganization);
        }

        foreach (var account in accounts)
        {
            var user = account == accounts[0]
                ? superAdmin
                : await FindOrCreateUserAsync(account, password, cancellationToken);
            var membership = await db.OrganizationMembers.SingleOrDefaultAsync(
                item => item.OrganizationId == systemOrganization.Id && item.UserId == user.Id,
                cancellationToken);
            if (membership is null)
                db.OrganizationMembers.Add(new OrganizationMember(systemOrganization.Id, user.Id, account.RoleId));
            else if (membership.RoleId != account.RoleId)
                membership.ChangeRole(account.RoleId);
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Count} development role users are ready in EventLens System", accounts.Length);
    }

    private async Task<User> FindOrCreateUserAsync(SeedAccount account, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = account.Email.ToUpperInvariant();
        var user = await db.Users.SingleOrDefaultAsync(
            item => item.NormalizedEmail == normalizedEmail, cancellationToken);
        if (user is not null)
        {
            user.ChangePassword(passwords.Hash(password));
            user.VerifyEmail();
            return user;
        }

        user = new User(account.FirstName, account.LastName, account.Email, passwords.Hash(password), null);
        user.VerifyEmail();
        db.Users.Add(user);
        return user;
    }

    private sealed record SeedAccount(string FirstName, string LastName, string Email, Guid RoleId);
}
