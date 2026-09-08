using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EventLensAI.Infrastructure.Persistence;

public sealed class EventLensDbContextFactory : IDesignTimeDbContextFactory<EventLensDbContext>
{
    public EventLensDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("EVENTLENSAI_SQLSERVER")
            ?? "Server=.;Database=EventLensAI;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True";
        return new EventLensDbContext(new DbContextOptionsBuilder<EventLensDbContext>()
            .UseSqlServer(connection).Options, null);
    }
}
