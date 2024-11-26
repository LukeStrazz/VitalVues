using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VitalVues.Models;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Services.ViewModels;
using System.Security.Claims;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/PaymentController")]
public class PaymentController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public PaymentController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [NoCacheHeaders]
    [HttpGet("Payment")]
    public IActionResult Payment()
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var user = _userService.FindUser(userUniqueIdentifier);
		return View(user);
    }

    [HttpGet("SuccessMonthly")]
    public async Task<IActionResult> SuccessMonthly() 
    { 
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var user = _userService.FindUser(userUniqueIdentifier);

        if (user != null)
        {
            user.IsSubscribed = true;
            if(user.SubscriptionStartDate == null)
            {
                user.SubscriptionStartDate = DateTime.UtcNow;
            }
            user.SubscriptionEndDate = DateTime.UtcNow.AddMonths(1);

            _userService.UpdateUser(user);
        }

        return View("SuccessMonthly", user);
    }
}

