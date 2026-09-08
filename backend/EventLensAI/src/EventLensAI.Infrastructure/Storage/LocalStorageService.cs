using EventLensAI.Application.Features.Photos.PhotoProcessing;
using Microsoft.Extensions.Configuration;

namespace EventLensAI.Infrastructure.Storage;

internal sealed class LocalStorageService : IStorageService
{
    private readonly string root;
    public LocalStorageService(IConfiguration configuration)
    {
        root = Path.GetFullPath(configuration["Storage:LocalPath"] ?? Path.Combine(AppContext.BaseDirectory, "storage"));
        Directory.CreateDirectory(root);
    }
    public async Task<string> UploadAsync(Stream content, string fileName, string contentType, CancellationToken ct)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension is not (".jpg" or ".jpeg" or ".png" or ".webp" or ".svg" or ".gif" or ".pdf" or ".mp4" or ".mov" or ".webm" or ".doc" or ".docx" or ".xls" or ".xlsx" or ".csv" or ".txt" or ".zip")) throw new InvalidOperationException("Unsupported file type.");
        var key = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";
        var path = Resolve(key); Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
        await content.CopyToAsync(output, ct); return key;
    }
    public Task DeleteAsync(string storageKey, CancellationToken ct) { var path = Resolve(storageKey); if (File.Exists(path)) File.Delete(path); return Task.CompletedTask; }
    public string GetUrl(string storageKey) => $"/storage/{storageKey.Replace('\\', '/')}";
    public Task<Stream> OpenReadAsync(string storageKey, CancellationToken ct) =>
        Task.FromResult<Stream>(new FileStream(Resolve(storageKey), FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true));
    private string Resolve(string key)
    {
        key = key.Replace("/storage/", "", StringComparison.OrdinalIgnoreCase).TrimStart('/', '\\');
        var path = Path.GetFullPath(Path.Combine(root, key.Replace('/', Path.DirectorySeparatorChar)));
        if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new UnauthorizedAccessException("Invalid storage key.");
        return path;
    }
}
