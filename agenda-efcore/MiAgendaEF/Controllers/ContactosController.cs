using Microsoft.AspNetCore.Mvc;

namespace MiAgendaEF.Controllers
{
    public class ContactosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
