using Microsoft.AspNetCore.Mvc;

namespace Dressly_MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult Privacy() => View();
}
