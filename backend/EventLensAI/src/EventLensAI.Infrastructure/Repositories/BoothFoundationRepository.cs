using EventLensAI.Application.Features.Booth;
using EventLensAI.Domain.Entities;
using EventLensAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventLensAI.Infrastructure.Repositories;
internal sealed class BoothFoundationRepository(EventLensDbContext db):IBoothFoundationRepository
{
    public Task<BoothConfiguration?>GetConfigurationAsync(Guid organizationId,CancellationToken ct)=>db.BoothConfigurations.FirstOrDefaultAsync(x=>x.OrganizationId==organizationId,ct);
    public Task AddConfigurationAsync(BoothConfiguration value,CancellationToken ct)=>db.BoothConfigurations.AddAsync(value,ct).AsTask();
    public async Task<IReadOnlyList<BoothCapability>>GetCapabilitiesAsync(Guid organizationId,CancellationToken ct)=>await db.BoothCapabilities.AsNoTracking().Where(x=>x.OrganizationId==organizationId).OrderBy(x=>x.Name).ToListAsync(ct);
    public async Task<IReadOnlyList<BoothDevice>>GetDevicesAsync(Guid organizationId,CancellationToken ct)=>await db.BoothDevices.AsNoTracking().Where(x=>x.OrganizationId==organizationId).OrderBy(x=>x.Kind).ThenBy(x=>x.Label).ToListAsync(ct);
    public async Task<IReadOnlyList<BoothHealthCheck>>GetHealthAsync(Guid organizationId,CancellationToken ct)=>await db.BoothHealthChecks.AsNoTracking().Where(x=>x.OrganizationId==organizationId).OrderByDescending(x=>x.CheckedAt).Take(50).ToListAsync(ct);
    public Task<CameraPreference?>GetCameraPreferenceAsync(Guid organizationId,CancellationToken ct)=>db.CameraPreferences.FirstOrDefaultAsync(x=>x.OrganizationId==organizationId,ct);
    public Task AddCameraPreferenceAsync(CameraPreference value,CancellationToken ct)=>db.CameraPreferences.AddAsync(value,ct).AsTask();
}
