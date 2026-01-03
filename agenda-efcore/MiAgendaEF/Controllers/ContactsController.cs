using Agenda.EFCore.Controllers;
using Infrastructure.Contract;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiAgendaEF.Controllers;

[Authorize]
public class ContactsController : BaseController
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
            var usuarioId = GetCurrentUserId();
            var NombreUsuario = GetCurrentUserName();

            if (usuarioId == 0)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión.";
                return RedirectToAction("Login", "Account");
            }

            // Intento obtener los contactos de la capa infraestructura
            var contactos = await _miAgendaInfrastructure.GetContactByIdAsync(usuarioId);

            //Crea ViewModel
            var agendaViewModel = new AgendaViewModel
            {
                Titulo = $"Agenda de {NombreUsuario}",
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
    #endregion

    [HttpPost]
    public async Task<IActionResult> Create(CreateContactViewModel model)
    {
        try
        {
            var UsuarioId = GetCurrentUserId();

            if (UsuarioId == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var usuario = new CreateContactViewModel
            {
                Nombre = model.Nombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                FechaNacimiento = model.FechaNacimiento,
                FotoArchivo = model.FotoArchivo,
                Telefono = model.Telefono,
            };

            return View("Create");
        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update()
    {
        try
        {

        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        try
        {

        }
        catch (Exception)
        {

            throw;
        }
    } 
}
