using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WorkshopManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _log;

    public HomeController(ILogger<HomeController> log) => _log = log;

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}