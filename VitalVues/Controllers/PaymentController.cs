using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VitalVues.Models;
using Microsoft.AspNetCore.Identity;
using Services.Interfaces;
using Services.ViewModels;
using System.Security.Claims;
using Hangfire;
using System;
using Services.Services;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/PaymentController")]
public class PaymentController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserService _userService;
    private readonly SendGridEmailService _sendGridEmailService;
    private static Dictionary<string, string> paymentJobTracker = new Dictionary<string, string>();

    public PaymentController(ILogger<UserController> logger, IUserService userService, SendGridEmailService sendGridEmailService)
    {
        _logger = logger;
        _userService = userService;
        _sendGridEmailService = sendGridEmailService;
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

            var userEmail = user.Email;

            // Schedule an email reminder a month before the subscription ends
            var reminderTime = user.SubscriptionEndDate?.AddDays(-1);
            if (reminderTime.HasValue && reminderTime.Value > DateTime.UtcNow)
            {
                var jobIdReminder = BackgroundJob.Schedule(() => _sendGridEmailService.SendEmail(
                    userEmail,
                    "Subscription Reminder",
                    "Your subscription is about to expire",
                    "<p>Your subscription will expire in one day.</p>" +
                    "<p>Please renew your subscription to continue enjoying our services.</p>"
                ), reminderTime.Value - DateTime.UtcNow);

                // Store the job ID for the reminder email
                paymentJobTracker[$"{user.Id}_subscriptionReminder"] = jobIdReminder;
            }
        }

        return View("SuccessMonthly", user);
    }
}