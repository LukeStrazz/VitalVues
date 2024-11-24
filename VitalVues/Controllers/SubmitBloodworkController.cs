using Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;
using UglyToad.PdfPig;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using Hangfire;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/SubmitBloodwork")]
public class SubmitBloodworkController : Controller
{
    private readonly ILogger<SubmitBloodworkController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IChatService _chatService;
    private readonly IBloodworkService _bloodworkService;
    private readonly IUserService _userService;
    private readonly SendGridEmailService _sendGridEmailService;

    public SubmitBloodworkController(
        ILogger<SubmitBloodworkController> logger,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IChatService chatService,
        IBloodworkService bloodworkService,
        IUserService userService,
        SendGridEmailService sendGridEmailService)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _chatService = chatService;
        _bloodworkService = bloodworkService;
        _userService = userService;
        _sendGridEmailService = sendGridEmailService;
    }

    [NoCacheHeaders]
    [HttpGet("SubmitBloodwork")]
    public IActionResult SubmitBloodwork()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("SignIn", "Account");
        }

        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var user = _userService.FindUser(userUniqueIdentifier);

        if (user.SubscriptionEndDate == null || user.SubscriptionEndDate <= DateTime.Now.Date)
        {
            return RedirectToAction("PaymentRequired", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadPdf(IFormFile pdfFile)
    {
        if (pdfFile == null || pdfFile.Length == 0)
            return Json(new { success = false, message = "No file selected or file is empty" });

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, pdfFile.FileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await pdfFile.CopyToAsync(fileStream);
        }

        var pdfContentList = new List<string>();
        using (PdfDocument document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                pdfContentList.Add(page.Text);
            }
        }

        string combinedPdfContent = string.Join("\n", pdfContentList);

        var testResults = ExtractTestResults(combinedPdfContent);

        // Delete the temporary PDF file after extraction
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        return Json(new { success = true, message = "File uploaded successfully", content = testResults });
    }

    public List<TestResultViewModel> ExtractTestResults(string combinedPdfContent)
    {
        string pattern = @"(?<TestName>[A-Z ,\d\-\/]+(?:,\d+[A-Z]*)*)\s*(?<Result>\d+(\.\d+)?)\s*(?<Grade>[HL])?\s*Reference Range:";

        // Extract matches
        var matches = Regex.Matches(combinedPdfContent, pattern);
        var testResults = matches
            .Select(match => new TestResultViewModel
            {
                TestName = match.Groups["TestName"].Value.Trim(),
                Result = match.Groups["Result"].Value.Trim(),
                Grade = match.Groups["Grade"].Success ? match.Groups["Grade"].Value.Trim() : string.Empty // Capture grade if it exists
            })
            .ToList();


        return testResults;
    }

    [HttpPost("SubmitResults")]
    public IActionResult SubmitResults([FromBody] SubmitBloodworkRequest request)
    {
        if (request.Tests == null || !request.Tests.Any())
        {
            return BadRequest("The tests field is required.");
        }

        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        _bloodworkService.AddBloodwork(request.Tests, userUniqueIdentifier, request.SubmissionDate);

        return Json(new { success = true });
    }

    public IActionResult ChatConnector(string content)
    {
        var apiKey = _configuration["API_KEY"];

        if (apiKey == null)
        {
            return Json(new { error = false, message = "Could not connect to services right now." });
        }

        var response = _chatService.GetChatResponse(apiKey, content);

        if (response == null)
        {
            string errorResponse = "Unable to get response.";
            return Json(new { success = false, message = errorResponse });
        }

        return Json(new { success = false, message = response });
    }

    [HttpPost("SaveReminders")]
    public async Task<IActionResult> SaveReminders([FromBody] Dictionary<string, bool> reminderSelections)
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        // Get user email from the UserService
        var user = _userService.FindUser(userUniqueIdentifier);
        if (user == null || string.IsNullOrEmpty(user.Email))
        {
            return BadRequest("User email not found.");
        }

        var userEmail = user.Email;

        // Get the current date 
        var currentDate = DateTime.Now.AddHours(4);

        // Daily reminder
        if (reminderSelections.ContainsKey("Daily") && reminderSelections["Daily"])
        {

            RecurringJob.AddOrUpdate($"daily-email-{userUniqueIdentifier}",
                () => _sendGridEmailService.SendEmail(userEmail, "Daily Reminder", "This is your daily reminder to upload your bloodwork.",
                "This is your daily reminder to upload your bloodwork."),
                Cron.Daily(currentDate.Hour, currentDate.Minute)); 
        }
        else
        {
            RecurringJob.RemoveIfExists($"daily-email-{userUniqueIdentifier}");
        }

        // Weekly reminder
        if (reminderSelections.ContainsKey("Weekly") && reminderSelections["Weekly"])
        {

            RecurringJob.AddOrUpdate($"weekly-email-{userUniqueIdentifier}",
                () => _sendGridEmailService.SendEmail(userEmail, "Weekly Reminder", "This is your weekly reminder to upload your bloodwork.",
                "This is your weekly reminder to upload your bloodwork."),
                Cron.Weekly(currentDate.DayOfWeek, currentDate.Hour, currentDate.Minute)); 
        }
        else
        {
            RecurringJob.RemoveIfExists($"weekly-email-{userUniqueIdentifier}");
        }

        // Monthly reminder
        if (reminderSelections.ContainsKey("Monthly") && reminderSelections["Monthly"])
        {

            RecurringJob.AddOrUpdate($"monthly-email-{userUniqueIdentifier}",
                () => _sendGridEmailService.SendEmail(userEmail, "Monthly Reminder", "This is your monthly reminder to upload your bloodwork.",
                "This is your monthly reminder to upload your bloodwork."),
                Cron.Monthly(currentDate.Day, currentDate.Hour, currentDate.Minute)); 
        }
        else
        {
            RecurringJob.RemoveIfExists($"monthly-email-{userUniqueIdentifier}");
        }

        // Yearly reminder
        if (reminderSelections.ContainsKey("Yearly") && reminderSelections["Yearly"])
        {

            RecurringJob.AddOrUpdate($"yearly-email-{userUniqueIdentifier}",
                () => _sendGridEmailService.SendEmail(userEmail, "Yearly Reminder", "This is your yearly reminder to upload your bloodwork.",
                "This is your yearly reminder to upload your bloodwork."),
                Cron.Yearly(currentDate.Month, currentDate.Day, currentDate.Hour, currentDate.Minute)); 
        }
        else
        {
            RecurringJob.RemoveIfExists($"yearly-email-{userUniqueIdentifier}");
        }

        return Ok(new { message = "Reminders updated successfully." });
    }



}
