using Infrastructure.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class LocalFileStorageService : ILocalFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
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

        // Retornar ruta relativa para guardar en BD
        return $"/uploads/{folder}/{fileName}";
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return Task.FromResult(false);
 
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
                return Task.FromResult(true);

            return Task.FromResult(false);
    }

    public string GetFileUrl(string filePath)
    {
        return string.IsNullOrEmpty(filePath) ? "/images/default.png" : filePath;
    }
}
