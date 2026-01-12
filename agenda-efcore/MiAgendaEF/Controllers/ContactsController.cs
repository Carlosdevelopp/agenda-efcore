using Agenda.EFCore.Controllers;
using DataAccess.Models.Tables;
using Infrastructure.Contract;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace MiAgendaEF.Controllers;

[Authorize]
public class ContactsController : BaseController
{
    private readonly IMiAgendaInfrastructure _miAgendaInfrastructure;
    private readonly ILocalFileStorageService _FileStorage;
    private readonly ILogger<ContactsController> _logger;

	public ContactsController(IMiAgendaInfrastructure miAgendaInfrastructure, ILocalFileStorageService fileStorage, ILogger<ContactsController> logger)
	{

		_miAgendaInfrastructure = miAgendaInfrastructure;
        _FileStorage = fileStorage;
        _logger = logger;
	}

    #region GET
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            var NombreUsuario = GetCurrentUserName();

            if (usuarioId == 0)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión.";
                return RedirectToAction("Login", "Account");
            }

            // Intento obtener los contactos de la capa infraestructura
            var contactos = await _miAgendaInfrastructure.GetContactsByUserIdAsync(usuarioId);

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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var UsuarioId = GetCurrentUserId();

            if (UsuarioId == 0)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión";
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Guardar foto si existe
            string? fotoRuta = null;
            if (model.FotoRuta != null && model.FotoRuta.Length > 0)
            {
                fotoRuta = await _FileStorage.SaveFileAsync(model.FotoRuta, "contactos");
            }

            // Crear entidad
            var nuevoContacto = new Contacto
            {
                Nombre = model.NombreCompleto,
                FechaNacimiento = model.FechaNacimiento,
                Telefono = model.Telefono,
                FotoRuta = fotoRuta, // Guardar la ruta
                UsuarioId = model.UsuarioId,
                FechaRegistro = DateTime.Now,
                Detalle = new List<DetalleContactoRed>()
            };

            if (!string.IsNullOrWhiteSpace(model.Instagram))
            {
                nuevoContacto.Detalle.Add(new DetalleContactoRed
                {
                    TipoContactoId = 1,
                    URL = NormalizarUrlRedSocial(model.Instagram, "instagram"),
                    FechaRegistro = DateTime.Now
                });
            }

            if (!string.IsNullOrWhiteSpace(model.Facebook))
            {
                nuevoContacto.Detalle.Add(new DetalleContactoRed
                {
                    TipoContactoId = 2,
                    URL = NormalizarUrlRedSocial(model.Facebook, "facebook"),
                    FechaRegistro = DateTime.Now
                });
            }

            if (!string.IsNullOrWhiteSpace(model.Twitter))
            {
                nuevoContacto.Detalle.Add(new DetalleContactoRed
                {
                    TipoContactoId = 3,
                    URL = NormalizarUrlRedSocial(model.Twitter, "twitter"),
                    FechaRegistro = DateTime.Now
                });
            }

            var result = await _miAgendaInfrastructure.CreateContactAsync(nuevoContacto);

            if (result != null)
            {
                TempData["SuccessMessage"] = "Contacto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "No se puede crear el contacto.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear contacto.");

            if (!string.IsNullOrEmpty(model.FotoRuta))
            {
                await _FileStorage.DeleteFileAsync(model.FotoRuta);
            }

            TempData["ErrorMessage"] = "Error al crear el contacto.";
            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Update(int contactoId)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión.";
                return RedirectToAction("Login", "Account");
            }

            var contactoExistente = await _miAgendaInfrastructure.GetContactByIdAsync(contactoId);

            if (contactoExistente == null)
            {
                TempData["ErrorMessage"] = "Contacto no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Validación de seguridad
            if (contactoExistente.UsuarioId != usuarioId)
            {
                TempData["ErrorMessage"] = "No tienes permiso para editar este contacto.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ContactoViewModel
            {
                ContactoId = contactoExistente.ContactoId,
                NombreCompleto = contactoExistente.Nombre,
                Telefono = contactoExistente.Telefono,
                FechaNacimiento = contactoExistente.FechaNacimiento,
                Edad = _miAgendaInfrastructure.CalcularEdad(contactoExistente.FechaNacimiento),
                FotoRuta = contactoExistente.FotoRuta,
                UsuarioId = contactoExistente.UsuarioId,
                RedesSociales = contactoExistente.Detalle?.Select(d => new RedSocialViewModel
                {
                    TipoContactoId = d.ContactoId,
                    URL = d.URL
                }).ToList() ?? new List<RedSocialViewModel>()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar contacto.");
            TempData["ErrorMessage"] = "Error al actualizar el contacto.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Update(int contactoId, Contacto model)
    {
        try
        {
            var result = await _miAgendaInfrastructure.UpdateContactAsync(model);

            if (result)
            {
                TempData["SuccessMessage"] = "Contacto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "No se pudo actualizar el contacto.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el contacto.");
            TempData["ErrorMessage"] = "Error al actualizar el contacto.";
            return View(model);
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int contactoId)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                TempData["ErrorMessage"] = "Necesitas iniciar sesión.";
                return RedirectToAction("Login", "Account");
            }

            var contacto = await _miAgendaInfrastructure.GetContactByIdAsync(contactoId);
            if (contacto == null)
                return NotFound();

            //Eliminar foto 
            if (!string.IsNullOrEmpty(contacto.FotoRuta))
            {
                await _FileStorage.DeleteFileAsync(contacto.FotoRuta);
            }

            var result = await _miAgendaInfrastructure.DeleteContactAsync(contactoId, usuarioId);

            if (result)
            {
                TempData["SuccessMessage"] = "Contacto eliminado exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No tienes permiso para eliminar este contacto.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ërror al eliminar contacto.");
            TempData["ErrorMessage"] = "Error al eliminar el contacto.";
            return RedirectToAction(nameof(Index));
        }
    } 
}
