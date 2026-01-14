namespace Infrastructure.Contract;

public interface IRedSocialHelper
{
    string? NormalizarUrlRedSocial(string input, string tipoRedSocial);

    int ObtenerTipoRedSocialId(string tipoRedSocial);

    string? ExtraerUsername(string url);

    bool EsUsernameValido(string username);

    string ValidarYNormalizarUrl(string url, string tipoRedSocial);

    string[] ObtenerDominiosValidos(string tipoRedSocial);
}
