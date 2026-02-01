using Infrastructure.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class LocalFileStorageService : ILocalFileStorageService
{
  private readonly IWebHostEnvironment _environment;
  private readonly ILogger<LocalFileStorageService> _logger;

  public LocalFileStorageService(IWebHostEnvironment environment, ILogger<LocalFileStorageService> logger)
  {
      _environment = environment;
      _logger = logger;
  }
}

    public async Task<string?> SaveFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            return null;

        const long maxSize = 5 * 1024 * 1024;

        // Validar tamaño (5MB máximo)
        if (file.Length > maxSize)
            throw new InvalidOperationException("El archivo es demasiado grande (máximo 5MB)");

        // Validar extensión
        var allowedExtensions = new[] {".jpeg", ".jpg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Formato de archivo no permitido");

        var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };

        if (!allowedMimeTypes.Contains(file.ContentType))
            throw new InvalidOperationException("Tipe MIME no permitido.");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);

        // Crear directorio si no existe
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // Crear nombre único
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Guardar archivo
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        // Retornar ruta relativa para guardar en BD
        return $"/uploads/{folder}/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult(false);

        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            File.Delete(fullPath);

            _logger.LogInformation("Archivo eliminado: {FilePath}", filePath);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo: {FilePath}", filePath);
            return Task.FromResult(false);
        }
    }

    public string GetFileUrl(string filePath)
    {
        return string.IsNullOrEmpty(filePath) ? "/images/default.png" : filePath;
    }
}
