using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AiDietPlan.Models;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Services.ViewModels;

namespace AiDietPlan.Controllers;

[ApiController]
[Route("api/UserController")]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;

    public UserController(ILogger<UserController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("UserUpdate")]
    public IActionResult UserUpdate()
    {
        return View();
    }

    [HttpPost("UpdateUser")]
    public IActionResult UpdateUser([FromBody] UserInfoViewModel userInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (userInfo.FirstName == null)
        {
            return BadRequest("User information is null.");
        }

        _userService.UpdateUser(userInfo);

        return Ok(new
        {
            message = "User created successfully!"
            // TODO: Redirect to "Goals"
        });
    }
}

