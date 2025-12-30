using Infrastructure.Contract;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiAgendaEF.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly IMiAgendaInfrastructure _miAgendaInfrastructure;
    private readonly ILogger<ContactsController> _logger;

	public ContactsController(IMiAgendaInfrastructure miAgendaInfrastructure, ILogger<ContactsController> logger)
	{

		_miAgendaInfrastructure = miAgendaInfrastructure;
        _logger = logger;
	}

    #region GET
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            // Pon un breakpoint aquí y mira si "UsuarioId" está en la lista de keys.
            var usuarioIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var nombreUsuario = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(usuarioIdStr))
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión.";
                return RedirectToAction("Login", "Account");
            }

            //Convierte a int
            if (!int.TryParse(usuarioIdStr, out int usuarioId))
            {
                _logger.LogError("Usuario invalido: {usuarioIdStr}", usuarioIdStr);
                TempData["ErrorMessage"] = "Error al obtener tu usuario.";
                return RedirectToAction("Login", "Account");
            }
           
            // Intento obtener los contactos de la capa infraestructura
            var contactos = await _miAgendaInfrastructure.GetContactByIdAsync(usuarioId);

            //Crea ViewModel
            var agendaViewModel = new AgendaViewModel
            {
                Titulo = $"Agenda de {nombreUsuario}",
                TotalContactos = contactos?.Count ?? 0,
                Contactos = contactos?.Select(u => new ContactoViewModel
                {
                    ContactoId = u.ContactoId,
                    NombreCompleto = $"{u.Nombre} {u.PrimerApellido}",
                    Telefono = u.Telefono,
                    Edad = _miAgendaInfrastructure.CalcularEdad(u.FechaNacimiento),
                    RedesSociales = u.Detalle?.Select(d => new RedSocialViewModel 
                    {
                        URL = d.URL
                    }).ToList() ?? new List<RedSocialViewModel>()
                }).ToList() ?? new List<ContactoViewModel>()
            };

            return View("Agenda", agendaViewModel);
        }
        catch (Exception ex)
        {
            // Si algo falla, redirigimos a una página de error o al Login con un mensaje

            _logger.LogError(ex, "Error al cargar agenda del usuario.");
            TempData["ErrorMessage"] = "No se pudieron cargar tus contactos. Por favor, reintenta.";
            return RedirectToAction("Login", "Account");
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            return View("Create");
        }
        catch (Exception)
        {

            throw;
        }
    }
    #endregion
}
