using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Threading.Tasks;

namespace VitalVues.Controllers
{
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

        [HttpGet]
        public IActionResult Email()
        {
            return View("~/Views/Notification/SendGridEmail.cshtml"); // Render the email form
        }
    }
}


