
namespace Andromeda.Common.Services.FileStorage;

public interface IFileStorageService
{
    Task<Result<string>> SaveProfileImageAsync(Guid userId, IFormFile file, CancellationToken ct = default);
    void DeleteProfileImage(string relativePath);
}