using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace loginform_with_database.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
