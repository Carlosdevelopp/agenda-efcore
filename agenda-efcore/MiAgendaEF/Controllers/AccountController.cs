using Agenda.EFCore.Models.ViewModels;
using DataAccess.Models.Tables;
using Infrastructure.Contract;
using MiAgendaEF.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiAgendaEF.Controllers;

public class AccountController : Controller
{
    private readonly IMiAgendaInfrastructure _miAgendaInfrastructure;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IMiAgendaInfrastructure miAgendaInfrastructure, ILogger<AccountController> logger)
    {
        _miAgendaInfrastructure = miAgendaInfrastructure;
        _logger = logger;
    }

    public IActionResult Index()
    {
        try
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Contacts");
            }
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Index de Account");
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        try
        {
            //Si  ya está autenticado, redirige
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Contacts");
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar Login");
            return View();
        }

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

            //Crear claims (información del usuario)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                //Claim personalizado
                new Claim("FullName", usuario.NombreUsuario)
            };

            //Crear identidad
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //Propiedades de autenticación
            var authProperties = new AuthenticationProperties
            {
                //Cookie persistente
                IsPersistent = model.Recordarme,
                ExpiresUtc = model.Recordarme ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2),
                IssuedUtc = DateTimeOffset.UtcNow
            };

            //Iniciar sesión
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            //Redirigir URL de retorno
            var returnUrL = Request.Query["returnUrl"].ToString();
            if (!string.IsNullOrEmpty(returnUrL) && Url.IsLocalUrl(returnUrL))
                return Redirect(returnUrL);

            return RedirectToAction("Index", "Contacts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en login para usuario: {Credencial}", model.Credencial);
            ModelState.AddModelError("", "Error al iniciar sesión. Intenta nuevamente.");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        try
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Contacts");
            }
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar Register.");
            return View();
        }

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

            var result = await _miAgendaInfrastructure.RegisterUserAsync(usuario);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["SuccessMessage"] = "Registro exitoso. Inicia sesión.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception)
        {
            ModelState.AddModelError("", "Ocurrio un error al registrar el usuario.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar sesión");
            return RedirectToAction(nameof(Login));
        }
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] = "El enlace es inválido o ha expirado.";
                return RedirectToAction(nameof(Login));
            }
               
            var model = new ResetPasswordViewModel
            {
                Token = token
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar ResetPassword");
            TempData["ErrorMessage"] = "Ocurrió un error. Intenta nuevamente.";
            return RedirectToAction(nameof(Login));
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _miAgendaInfrastructure.ResetPasswordAsync(model.Token, model.NewPassword);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            TempData["SuccessMessage"] = "Contraseña restablecida exitosamente. Inicia sesión.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ResetPassword");
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        try
        {
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar ForgotPassword");
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _miAgendaInfrastructure.ForgotPasswordAsync(model.Email);
            TempData["InfoMessage"] = "Si el correo existe, recibirás instrucciones para restablecer tu contraseña.";

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ForgotPassword para: {Email}", model.Email);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        try
        {
            _logger.LogWarning("Acceso denegado para usuario: {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar AccessDenied");
            return View();
        }
    }
}
