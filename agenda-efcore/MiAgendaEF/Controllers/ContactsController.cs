using Infrastructure.Contract;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MiAgendaEF.Controllers;

public class ContactsController : Controller
{
    private readonly IMiAgendaInfrastructure _miAgendaInfrastructure;

	public ContactsController(IMiAgendaInfrastructure miAgendaInfrastructure)
	{
		_miAgendaInfrastructure = miAgendaInfrastructure;
	}

    #region GET
    public async Task<IActionResult> Index()
    {
        try
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (string.IsNullOrEmpty(usuarioIdStr)) return RedirectToAction("Login", "Account");

            int usuarioId = int.Parse(usuarioIdStr);
            var nombreUsuario = HttpContext.Session.GetString("UsuarioNombre");

            // Intento obtener los contactos de la infraestructura
            var contactos = await _miAgendaInfrastructure.GetContactByIdAsync(usuarioId);

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
                    RedesSociales = u.Detalle?.Select(d => new RedSocialViewModel { URL = d.URL }).ToList()
                                    ?? new List<RedSocialViewModel>()
                }).ToList() ?? new List<ContactoViewModel>()
            };

            return View("Agenda", agendaViewModel);
        }
        catch (Exception)
        {
            // Si algo falla, redirigimos a una página de error o al Login con un mensaje
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
