using DataAccess.Models.Tables;
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
        if (!ModelState.IsValid) return View(model);

        try
        {
            var usuario = await _miAgendaInfrastructure.LoginAsync(model.Credencial, model.Password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Guardar datos de sesión
            HttpContext.Session.SetString("UsuarioId", usuario.UsuarioId.ToString());
            HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);

            return RedirectToAction("Index", "Contacts");
        }
        catch (Exception)
        {
            // Aquí podrías loguear el error: _logger.LogError(ex.Message);
            ModelState.AddModelError("", "Error al iniciar sesión.");
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                PrimerApellido = model.PrimerApellido,
                SegundoApellido = model.SegundoApellido,
                Correo = model.Correo,
                NombreUsuario = model.NombreUsuario,
                Password = model.Password,
                Telefono = model.Telefono,
            };

            var result = await _miAgendaInfrastructure.RegisterAsync(usuario);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(model);
            }

            TempData["success"] = result.Message;
            return RedirectToAction("Login");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Ocurrio un error al registrar  el  usuario.");
            return View(model);
        }
    }
}
