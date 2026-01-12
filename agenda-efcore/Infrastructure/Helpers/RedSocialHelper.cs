namespace Infrastructure.Helpers;

public class RedSocialHelper
{
    public string? NormalizarUrlRedSocial(string input, string tipoRedSocial)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        input = input.Trim();

        // Validarla y normalizarla  
        if (input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || input.StartsWith("https://",StringComparison.OrdinalIgnoreCase))
        {
            return ValidarYNormalizarUrl(input, tipoRedSocial);
        }

        // Remover @ si existe
        var username = input.StartsWith("@") ? input.Substring(1) : input;

        // Validar username
        if (!EsUsernameValido(username))
            throw new ArgumentException($"Username inválido: {username}"); 

        //Construir URL según el tipo de red social
        return tipoRedSocial.ToLower() switch
        {
            "instagram" => $"https://instagram.com/{username}",
            "facebook" => $"https://facebook.com/{username}",
            "twitter" => $"https://twitter.com/{username}",
            _ => throw new ArgumentException($"Tipo de red social no soportado: {tipoRedSocial}")
        };
    }

    public static int ObtenerTipoRedSocialId(string  tipoRedSocial)
    {
        return tipoRedSocial.ToLower() switch
        {
            "instagram" => 1,
            "facebook" => 2,
            "twitter" => 3,
            _ => throw new ArgumentException($"Tipo  de  red  social  no reconocido:{tipoRedSocial}")
        };
    }

    public static string? ExtraerUsername(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        if (!url.Contains("/"))
            return url.TrimStart('@');

        var uri = new Uri(url);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 0 ? segments[0] : null;
    }

    private static bool EsUsernameValido(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_.]+$") && username.Length >= 1 && username.Length <= 30;
    }

    private static string ValidarYNormalizarUrl(string url,string tipoRedSocial)
    {
        try
        {
            var uri = new Uri(url);
            var dominiosValidos = ObtenerDominiosValidos(tipoRedSocial);

            if (!dominiosValidos.Any(d => uri.Host.Equals(d, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"La URL no pertenece a {tipoRedSocial}");
            }

            return url.Replace("http://", "https://");
        }
        catch (UriFormatException)
        {
            throw new ArgumentException("Formato de URL inválido");
        }
    }  

    private static string[] ObtenerDominiosValidos(string tipoRedSocial)
    {
        return tipoRedSocial.ToLower() switch
        {
            "instagram" => new[] {"instagram.com", "www.instagram.com"},
            "facebook" => new[] {"facebook.com", "www.facebook.com", "fb.com"},
            "twitter" => new[] {"twitter.com", "www.twitter.com"},
            _ => Array.Empty<string>()
        };
    }
}

