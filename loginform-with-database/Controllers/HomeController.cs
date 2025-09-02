using Microsoft.AspNetCore.Mvc;

namespace loginform_with_database.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
