using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using VVData.Data;
using Hangfire;
using System;
using Services.Services;
using System.Collections.Generic;

[ApiController]
[Route("api/FastingController")]
public class FastingController : Controller
{
    private readonly IFastingService _fastService;
    private readonly DatabaseContext _context;
    private readonly SendGridEmailService _sendGridEmailService;
    private readonly IUserService _userService;

    // In-memory dictionary to store the job IDs associated with users
    private static Dictionary<string, string> fastingJobTracker = new Dictionary<string, string>();

    public FastingController(IFastingService fastService, DatabaseContext context, SendGridEmailService sendGridEmailService, IUserService userService)
    {
        _fastService = fastService;
        _context = context;
        _sendGridEmailService = sendGridEmailService;
        _userService = userService;
    }

    [HttpPost("SaveTime")]
    public IActionResult SaveTime([FromBody] FastingViewModel model)
    {
        if (model == null)
        {
            return BadRequest(new { success = false, message = "Model is null" });
        }

        if (ModelState.IsValid)
        {
            // Check if an active fasting session already exists
            var existingTimer = _fastService.GetLatestFastingSession(model.userID);
            if (existingTimer != null && existingTimer.end > DateTime.Now)
            {
                return BadRequest(new { success = false, message = "A fasting session is already running." });
            }

            // Save the new fasting session
            _fastService.SaveFast(model);

            // Find user and get email
            var user = _userService.FindUser(model.userID);
            if (user == null || string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { success = false, message = "User not found or email is missing." });
            }

            var userEmail = user.Email;
            var fastingDuration = model.end - model.start;

            // Schedule the 10-minute warning email if fasting duration is more than 10 minutes
            if (fastingDuration.TotalMinutes > 10)
            {
                var notificationTime = model.end.AddMinutes(-10).AddHours(-4);
     
                // Schedule the Hangfire job for the 10-minute warning
                var jobIdWarning = BackgroundJob.Schedule(() => _sendGridEmailService.SendEmail(
                    userEmail,
                    "Fasting Timer Alert",
                    $"Your fasting timer will end in 10 minutes.",
                    $"<p>Your fasting timer will end in 10 minutes.</p>"
                ), notificationTime - DateTime.Now);

                // Store the job ID for the 10-minute warning
                fastingJobTracker[$"{model.userID}_10min"] = jobIdWarning;
            }

            // Schedule the Hangfire job to send an email when the fasting timer ends
            var endTime = model.end.AddHours(-4);
            var formattedStartTime = model.start.AddHours(-4).ToString("f");
            var formattedEndTime = endTime.ToString("f");

            var jobIdEnd = BackgroundJob.Schedule(() => _sendGridEmailService.SendEmail(
                userEmail,
                "Fasting Timer Complete",
                "Congratulations! Your fasting timer has ended.",
                $"<p>Congratulations! Your fasting timer has successfully ended.</p>" +
                $"<p><strong>Summary:</strong></p>" +
                $"<p>Start time: <strong>{formattedStartTime}</strong></p>" +
                $"<p>End time: <strong>{formattedEndTime}</strong></p>"
            ), endTime - DateTime.Now);


            // Store the job ID for the fasting end email
            fastingJobTracker[$"{model.userID}_end"] = jobIdEnd;

            return Ok(new { success = true, message = "Fasting time saved successfully!", startTime = model.start });
        }

        return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
    }

    [HttpPost("ResetFastingSession")]
    public IActionResult ResetFastingSession([FromQuery] string userID)
    {
        // Check and delete the 10-minute warning job if it exists
        if (fastingJobTracker.ContainsKey($"{userID}_10min"))
        {
            var jobIdWarning = fastingJobTracker[$"{userID}_10min"];
            BackgroundJob.Delete(jobIdWarning);
            fastingJobTracker.Remove($"{userID}_10min");
        }

        if (fastingJobTracker.ContainsKey($"{userID}_end"))
        {
            var jobIdEnd = fastingJobTracker[$"{userID}_end"];
            BackgroundJob.Delete(jobIdEnd);
            fastingJobTracker.Remove($"{userID}_end");
        }

        // Reset the fasting session
        _fastService.ResetFastingSession(userID);

        return Ok(new { success = true, message = "Fasting session reset successfully" });
    }

    [HttpGet("GetFastingSession")]
    public IActionResult GetFastingSession([FromQuery] string userID)
    {
        var fastingSession = _fastService.GetLatestFastingSession(userID);

        if (fastingSession == null)
        {
            return NotFound(new { success = false, message = "No fasting session found" });
        }

        var now = DateTime.Now;
        var remainingTime = (fastingSession.end - now).TotalSeconds;

        return Ok(new
        {
            success = true,
            remainingTime = remainingTime > 0 ? remainingTime : 0 // Ensure non-negative time
        });
    }

    [HttpGet("Fasting")]
    public IActionResult Fasting()
    {
        return View();
    }
}
