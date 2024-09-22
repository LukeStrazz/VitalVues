using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using VVData.Data;

[ApiController]
[Route("api/FastingController")]
public class FastingController : Controller
{
    private readonly IFastingService _fastService;
    private readonly DatabaseContext _context;

    public FastingController(IFastingService fastService, DatabaseContext context)
    {
        _fastService = fastService;
        _context = context;
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

            // If no active session exists, proceed to save the new fasting session
            _fastService.SaveFast(model);
            return Ok(new { success = true, message = "Fasting time saved successfully!", startTime = model.start });
        }

        return BadRequest(new { success = false, message = "Invalid data", errors = ModelState });
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

    [HttpPost("ResetFastingSession")]
    public IActionResult ResetFastingSession([FromQuery] string userID)
    {
        _fastService.ResetFastingSession(userID);
        return Ok(new { success = true, message = "Fasting session reset successfully" });
    }

    [HttpGet("Fasting")]
    public IActionResult Fasting()
    {
        return View();
    }
}
