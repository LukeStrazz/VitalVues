using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AiDietPlan.Models;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Services.ViewModels;

namespace AiDietPlan.Controllers;

[ApiController]
[Route("api/UserCreateController")]
public class UserCreateController : Controller
{
    private readonly ILogger<UserCreateController> _logger;
    private readonly IUserService _userService;

    public UserCreateController(ILogger<UserCreateController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("CreateUser")]
    public IActionResult UserCreate()
    {
        return View();
    }

    [HttpPost("CreateUser")]
    public IActionResult CreateUser([FromBody] UserInfoViewModel userInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (userInfo.FirstName == null)
        {
            return BadRequest("User information is null.");
        }

        _userService.createUser(userInfo);

        return Ok(new
        {
            message = "User created successfully!"
            // TODO: Redirect to "Goals"
        });
    }
}

