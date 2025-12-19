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
