using Microsoft.AspNetCore.Mvc;
using Services.Interfaces; // Make sure you have the correct namespace for IMailService

public class NotificationController : Controller
{
    private readonly IMailService _mailService;

    // Constructor to inject the MailService
    public NotificationController(IMailService mailService)
    {
        _mailService = mailService;
    }

    // This method handles the form submission and sends the email
    [HttpPost]
    public IActionResult SendNotification(string message)
    {
        string toEmail = "justinpwhite17@gmail.com"; // This is the fixed email address where you send the message

        _mailService.SendEmail(
            toEmail,                                // The recipient's email
            "",                 // Your API key
            "", // Your Mailgun domain
            "User Notification",                    // Email subject
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




