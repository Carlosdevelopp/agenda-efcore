using Infrastructure.Contract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(IWebHostEnvironment environment,  IHttpContextAccessor httpContextAccessor)
    {
        _environment = environment;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folder)
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

        //Crear nombre único
        var fileName = $"{Guid.NewGuid()}{extension}";

        //Ruta en wwwwroot
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);

        //Crear directorio si no existe
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, fileName);

        //Guardar archivo
        using(var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{folder}/{fileName}";
    }
}
