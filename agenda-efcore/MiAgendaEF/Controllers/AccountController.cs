using Infrastructure.Contract;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MiAgendaEF.Controllers;

public class AccountController : Controller
{
    private readonly IMiAgendaInfrastructure _miAgendaInfrastructure;

    public AccountController(IMiAgendaInfrastructure miAgendaInfrastructure)
    {

        _miAgendaInfrastructure = miAgendaInfrastructure;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var usuario = await _miAgendaInfrastructure.LoginAsync(model.Credencial, model.Password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o  contraseña incorrectos");
                return View(model);
            }

            var contactos = await _miAgendaInfrastructure.GetContactByIdAsync(usuario.UsuarioId);

            var agendaViewModel = new AgendaViewModel
            {
                Titulo = $"Agenda de {usuario.Nombre}",
                TotalContactos = contactos.Count,
                Contactos = contactos.Select(u => new ContactoViewModel
                {
                    ContactoId = u.ContactoId,
                    NombreCompleto = $"{u.Nombre} {u.PrimerApellido}",
                    Telefono = u.Telefono,
                    Edad = _miAgendaInfrastructure.CalcularEdad(u.FechaNacimiento),
                    RedesSociales = u.Detalle.Select(d => new RedSocialViewModel
                    {
                        //NombreTipo = d.NombreUsuarioRed,
                        // Si tu RedSocialViewModel tiene un campo 'Valor' que mapea a la URL, úsalo:
                        URL = d.URL,

                        // Asumo que el campo URL en el ViewModel se llama 'Valor'

                        // Agregar otros campos de RedSocialViewModel si existen
                        //NombreUsuarioRed = d.NombreUsuarioRed ?? "N/A"
                    }).ToList()
                }).ToList()
            };

            HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
            HttpContext.Session.SetString("UsuarioNombre", usuario.NombreUsuario);

            if (model.Recordarme)
            {
                Response.Cookies.Append("UsuarioRecordado", usuario.NombreUsuario, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                });
            }

            return View("Agenda", agendaViewModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Ocurrió un error al intentar iniciar sesión. Inténtalo nuevamente.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Logout()
    {
        try
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("Usuario Recordado");
            return View("Login");
        }
        catch (Exception)
        {
            return RedirectToAction("Login");
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        try
        {
            return View("Register");
        }
        catch (Exception)
        {
            return RedirectToAction("Login");
        }
    }
}
