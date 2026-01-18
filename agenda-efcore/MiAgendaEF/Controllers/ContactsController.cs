using Agenda.EFCore.Controllers;
using Infrastructure.Contract;
using Infrastructure.DTOs;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            var contactosViewModel = contactos.Select(u => new ContactoViewModel
            {
                ContactoId = u.ContactoId,
                NombreCompleto = u.Nombre,
                Telefono = u.Telefono,
                Edad = _miAgendaInfrastructure.CalcularEdad(u.FechaNacimiento),
                FotoRuta = u.FotoRuta,

                RedesSociales = u.Detalle?.Select(d => new RedSocialViewModel
                {
                    TipoContactoId = d.TipoContactoId,
                    URL = d.URL
                }).ToList() ?? new List<RedSocialViewModel>()

            }).ToList();

            var model = new AgendaViewModel
            {
                Contactos = contactosViewModel,
                TotalContactos = contactosViewModel.Count
            };

            return View("Agenda",model); 
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
    public async Task<IActionResult> Create(CreateContactViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                TempData["ErrorMessage"] = "Debes iniciar sesión";
                return RedirectToAction("Login", "Account");
            }

            string? fotoRuta = null; 
            if (model.FotoRuta != null && model.FotoRuta.Length > 0)
            {
                fotoRuta = await _FileStorage.SaveFileAsync(model.FotoRuta, "contactos");
            }

            var dto = new CrearContactoDto
            {
                Nombre = model.Nombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                Telefono = model.Telefono,
                FechaNacimiento = model.FechaNacimiento,
                FotoRuta = fotoRuta,
                Instagram = model.Instagram,
                Facebook = model.Facebook,
                Twitter = model.Twitter
            };

            var contacto = await _miAgendaInfrastructure.CreateContactAsync(dto, usuarioId);

            TempData["SuccessMessage"] = "Contacto creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error de validación  al crear contacto.");
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear contacto.");
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

                Instagram = contactoExistente.Detalle?.FirstOrDefault(d => d.TipoContactoId == 1)?.URL,
                Facebook = contactoExistente.Detalle?.FirstOrDefault(d => d.TipoContactoId == 2)?.URL,
                Twitter = contactoExistente.Detalle?.FirstOrDefault(d => d.TipoContactoId == 3)?.URL
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar contacto para editar.");
            TempData["ErrorMessage"] = "Error al cargar el contacto.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int contactoId, ContactoViewModel model)
    {
        if (contactoId != model.ContactoId)
            return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var  usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                TempData["ErrorMeessage"] = "Debes iniciar sesión";
                return RedirectToAction("Login", "Account");
            }

            string? nuevaFotoRuta = model.FotoRuta;

            if (model.FotoPerfil != null && model.FotoPerfil.Length > 0 )
            {
                if (!string.IsNullOrEmpty(model.FotoRuta))
                {
                    await _FileStorage.DeleteFileAsync(model.FotoRuta);
                }

                nuevaFotoRuta = await _FileStorage.SaveFileAsync(model.FotoPerfil, "contactos");
            }

            var dto  = new ActualizarContactoDto
            {
                ContactoId = model.ContactoId,
                NombreCompleto = model.NombreCompleto,
                Telefono = model.Telefono,
                FechaNacimiento = model.FechaNacimiento,
                FotoRuta = nuevaFotoRuta,
                Instagram = model.Instagram,
                Facebook = model.Facebook,
                Twitter = model.Twitter
            };

            var updateResult = await _miAgendaInfrastructure.UpdateContactAsync(dto, usuarioId);

            TempData["SuccessMessage"] = "Contacto actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Contacto {ContactoId} no encontrado", contactoId);
            TempData["ErrorMessage"] = "El contacto no existe.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Usuario sin permisos para editar contacto {ContactoId}", contactoId);
            TempData["ErrorMessage"] = "No tienes permiso para editar este contacto.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar contacto");
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar contacto {ContactoId}", contactoId);
            TempData["ErrorMessage"] = "Ocurrió un error inesperado al actualizar el contacto.";
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
            if (contacto != null && contacto.UsuarioId == usuarioId)
            {
                if (!string.IsNullOrEmpty(contacto.FotoRuta))
                {
                    await _FileStorage.DeleteFileAsync(contacto.FotoRuta);
                }

                var result = await _miAgendaInfrastructure.DeleteContactAsync(contactoId, usuarioId);

                TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                    ? "Contacto eliminado exitosamente."
                    : "No tienes permiso para eliminar este contacto.";
            }

            TempData["ErrorMessage"] = "Contacto no encontrado o no tienes permiso para eliminarlo.";
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Sin permisos para eliminnar.");
            TempData["ErrorMessage"] = "No tienes permisos para eliminar este contacto.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aliminar");
            TempData["ErrorMessage"] = "Error al eliminar contacto.";
            return RedirectToAction(nameof(Index));
        }
    } 
}
