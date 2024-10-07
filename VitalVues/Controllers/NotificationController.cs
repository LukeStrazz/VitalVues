using Microsoft.AspNetCore.Mvc;
using Services.Interfaces; // Make sure you have the correct namespace for IMailService
namespace Services.Services;

public class NotificationController : Controller
{
    private readonly IMailService _mailService;
    private readonly IConfiguration _configuration;

    // Constructor to inject the MailService
    public NotificationController(IMailService mailService, IConfiguration configuration)
    {
        _mailService = mailService;
        _configuration = configuration;
    }

    // This method handles the form submission and sends the email
    [HttpPost]
    public IActionResult SendNotification(string message)
    {
        string toEmail = "justinpwhite17@gmail.com"; // This is the fixed email address where you send the message

        _mailService.SendEmail(
            toEmail,                                // The recipient's email
            _configuration["mailgun:Mailgun_API_Key"],
            _configuration["mailgun:Email_Domain"], // Your Mailgun domain
            "Customer Support Request",                    // Email subject
            message                                 // Message content
        );

        return Ok("Your message has been sent to support, Please give 1-2 days for a response."); // Return success message
    }

    // This method renders the email form
    [HttpGet]
    public IActionResult Email()
    {
        return View(); // This will render the email.cshtml form
    }
}




