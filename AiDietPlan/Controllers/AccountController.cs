using AiDietPlanData.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.ViewModels;
using System.ComponentModel;
using System.Security.Claims;

namespace AiDietPlan.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IUserService _userService;

    public AccountController(ILogger<AccountController> logger, IHttpContextAccessor httpContextAccessor, IUserService userService)
    {
        _logger = logger;
		_httpContextAccessor = httpContextAccessor;
        _userService = userService;
    }

    public IActionResult SignIn()
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult ExternalLogin(string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return RedirectToAction(nameof(SignIn));
        }

        var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

        if (result?.Principal == null)
        {
            return RedirectToAction(nameof(SignIn));
        }

		var email = _httpContextAccessor.HttpContext.User.Identity.Name ?? "N/A";

        // Check if the user exists
        try {

			var person = await _userService.UserExists(email);

            if(person != null)
            {
				return RedirectToLocal(returnUrl);
			}
		}
        catch (Exception ex)
        {
			return RedirectToAction("SignIn", "Account");
		}

		return RedirectToAction("SignIn", "Account");
	}


	private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

}