using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using VitalVues.Models;

namespace VitalVues.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [NoCacheHeaders]
    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("SignIn", "Account");
        }

        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var user = _userService.FindUser(userUniqueIdentifier);

        if (user.SubscriptionEndDate == null || user.SubscriptionEndDate <= DateTime.Now.Date)
        {
            return RedirectToAction("PaymentRequired", "Home");
        }

        return View();
    }


    [NoCacheHeaders]
    public IActionResult PaymentRequired()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("SignIn", "Account");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
