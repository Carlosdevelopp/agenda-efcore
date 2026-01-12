using Microsoft.AspNetCore.Http;

namespace Infrastructure.Contract;

public interface ILocalFileStorageService
{
    Task<string?> SaveFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    string GetFileUrl(string filePath);
}
