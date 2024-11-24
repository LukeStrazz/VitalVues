﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VitalVues.Models;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Services.ViewModels;
using System.Security.Claims;

namespace VitalVues.Controllers;

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

    [NoCacheHeaders]
    [HttpGet("UserUpdate")]
    public IActionResult UserUpdate()
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var user = _userService.FindUser(userUniqueIdentifier);

        if(user.SubscriptionEndDate <= DateTime.Now.Date)
        {
            return RedirectToAction("PaymentRequired", "Home");
        }

		return View(user);
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
            message = "User updated successfully!"
        });
    }
}

