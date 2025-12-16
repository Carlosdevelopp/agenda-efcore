using Infrastructure.Contract;
using Microsoft.AspNetCore.Mvc;

namespace MiAgendaEF.Controllers;

public class HomeController : Controller
{
    private readonly IMiAgendaInfrastructure _miAgendaInfrastructure;

    public HomeController(IMiAgendaInfrastructure miAgendaInfrastructure)
    {
        _miAgendaInfrastructure = miAgendaInfrastructure;
    }

    [HttpGet]
    public async Task<IActionResult> Inicio()
    {
        try
        {
            var usuarios = await _miAgendaInfrastructure.GetAllUsersAsync();
            return View("Index", usuarios);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

}
