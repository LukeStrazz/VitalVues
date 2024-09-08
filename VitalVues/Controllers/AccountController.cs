using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Services.ViewModels;
using System.Linq;
using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using Services.Interfaces;

namespace VitalVues.Controllers;

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

	public IActionResult Login(string returnUrl = "/")
    {
        // If the user is already authenticated, no need to challenge
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("LoginCallback", new { returnUrl });
        }

        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(Url.Action("LoginCallback", new { returnUrl }))
            .Build();

        // This will redirect to Auth0 for login
        return Challenge(authenticationProperties, Auth0Constants.AuthenticationScheme);
    }


	[Authorize]
    public async Task<IActionResult> LoginCallback(string returnUrl = "/")
    {

        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

		if (string.IsNullOrEmpty(userUniqueIdentifier))
		{
			return RedirectToAction("Error", "Home");
		}

        foreach(Claim c in User.Claims)
        {
            Console.WriteLine(c.Value); 
        }

		try
		{

            var personExists = await _userService.UserExists(userUniqueIdentifier);

            if (personExists == false)
            {
                var info = new UserInfoViewModel
                {
                    Sid = userUniqueIdentifier,
                    FirstName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                    LastName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                    Username = User.Identity.Name,
                    Email = "user@example.com",
                    ProfileImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                    Age = 0,
                    Birthday = DateTime.MinValue,
                    StartingWeight = 0,
                    CurrentWeight = 0,
                    Allergies = null,
                    Goals = null,
                };

                await _userService.CreateUser(info);
                RedirectToAction("UserUpdate", "User");
            }
            else
            {
                var person = _userService.FindUser(userUniqueIdentifier);
                var profImage = User.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;

                person.ProfileImage = profImage;

                _userService.UpdateUser(person);
            }

            return LocalRedirect(returnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError("Something went wrong during login or creation of user: {ex}", ex);
            return RedirectToAction("Error", "Home");
        }
    }

    [Authorize]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            // Indicate here where Auth0 should redirect the user after a logout.
            // Note that the resulting absolute Uri must be whitelisted in 
            .WithRedirectUri(Url.Action("Index", "Home"))
            .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// This is just a helper action to enable you to easily see all claims related to a user. It helps when debugging your
    /// application to see the in claims populated from the Auth0 ID Token
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public IActionResult Claims()
    {
        return View();
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}