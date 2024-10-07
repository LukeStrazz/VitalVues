using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Threading.Tasks;

namespace VitalVues.Controllers;

public class SendGridNotificationController : Controller
{
    private readonly SendGridEmailService _sendGridEmailService;

    public SendGridNotificationController(SendGridEmailService sendGridEmailService)
    {
        _sendGridEmailService = sendGridEmailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendEmail(string toEmail, string subject, string plainTextContent, string htmlContent)
    {
        await _sendGridEmailService.SendEmail(toEmail, subject, plainTextContent, htmlContent);
        return Ok("Email sent successfully.");
    }

    [NoCacheHeaders]
    [HttpGet]
    public IActionResult Email()
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        return View("~/Views/Notification/SendGridEmail.cshtml"); // Render the email form
    }
}


