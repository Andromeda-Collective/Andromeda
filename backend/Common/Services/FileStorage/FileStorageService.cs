namespace Andromeda.Common.Services.FileStorage;



public sealed class LocalFileStorageService : IFileStorageService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSizeBytes = 2 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<Result<string>> SaveProfileImageAsync(Guid userId, IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0)
            return FileStorageErrors.EmptyFile;

        if (file.Length > MaxFileSizeBytes)
            return FileStorageErrors.MaxFileSize;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
            return FileStorageErrors.FileFormat;

        var folder = Path.Combine(_environment.WebRootPath, "uploads", "profile-images");
        Directory.CreateDirectory(folder);

        var fileName = $"{userId:N}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(folder, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return $"/uploads/profile-images/{fileName}";
    }

    public void DeleteProfileImage(string relativePath)
    {
        if (relativePath == ProfileImageDefaults.DefaultImagePath)
            return;

        var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}