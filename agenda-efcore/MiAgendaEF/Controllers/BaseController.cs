using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Agenda.EFCore.Controllers;

public abstract class BaseController : Controller
{
    protected int GetCurrentUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdStr, out int userId) ? userId : 0;
    }

    protected string GetCurrentUserName()
    {
        return User.FindFirstValue(ClaimTypes.Name) ?? "Usuario";
    }
}
