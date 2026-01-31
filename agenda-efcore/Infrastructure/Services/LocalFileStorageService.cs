using Infrastructure.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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

    public async Task<string?> SaveFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            return null;

        // Validar extensión
        var allowedExtensions = new[] {".jpeg", ".jpg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("Formato de archivo no permitido");

        // Validar tamaño (5MB máximo)
        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("El archivo es demasiado grande (máximo 5MB)");

        // Crear nombre único
        var fileName = $"{Guid.NewGuid()}{extension}";

        // Ruta en wwwwroot
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);

        // Crear directorio si no existe
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        // Guardar archivo
        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        _logger.LogInformation($"Archivo guardado: {filePath}", filePath);

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

            if (File.Exists(fullPath))
                return Task.FromResult(true);

            File.Delete(fullPath);

            _logger.LogInformation("Archivo eliminado: {FilePath}", filePath);

            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo: {Filepath}", filePath);
            return Task.FromResult(false);
        }
    }

    public string GetFileUrl(string filePath)
    {
        return string.IsNullOrEmpty(filePath) ? "/images/default.png" : filePath;
    }
}
