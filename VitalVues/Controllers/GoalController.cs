using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using Hangfire;
using Services.Services;

namespace VitalVues.Controllers
{
    [ApiController]
    [Route("api/GoalController")]
    public class GoalController : Controller
    {
        private readonly IGoalService _goalService;
        private readonly IUserService _userService;
        private readonly SendGridEmailService _sendGridEmailService;
        private readonly ILogger<AccountController> _logger;

        public GoalController(ILogger<AccountController> logger, IGoalService goalService, IUserService userService, SendGridEmailService sendGridEmailService)
        {
            _logger = logger;
            _goalService = goalService;
            _userService = userService;
            _sendGridEmailService = sendGridEmailService;
        }

        [NoCacheHeaders]
        [HttpGet("Goal")]
        public IActionResult Goal()
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

            var goals = _goalService.GetGoals(userUniqueIdentifier).ToList();

            var userInfo = new UserInfoViewModel
            {
                Sid = userUniqueIdentifier,
                Goals = goals
            };

            return View(userInfo);
        }

        [HttpPost("CreateGoal")]
public IActionResult CreateGoal([FromBody] GoalViewModel goalInfo)
{
    // Check model validation
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        _logger.LogWarning("ModelState errors: " + string.Join(", ", errors));
        return BadRequest(ModelState);
    }

    // Check if goal description is null
    if (goalInfo.Description == null)
    {
        return BadRequest("Goal information is null.");
    }

    try
    {
        // Create the goal
        int createdGoalId = _goalService.CreateGoal(goalInfo);

        // Retrieve user info
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var userInfo = _userService.FindUser(userUniqueIdentifier);
        if (userInfo == null)
        {
            return BadRequest("User not found.");
        }

        // Calculate time span for goal duration
        var totalDuration = goalInfo.endGoalDate - goalInfo.startingGoalDate;

        // Calculate halfway point for the goal
        var halfwayPoint = goalInfo.startingGoalDate.Add(totalDuration / 2);

        // Schedule the halfway reminder email job
        var halfwayJobId = BackgroundJob.Schedule(() =>
            _sendGridEmailService.SendEmail(
                userInfo.Email,
                "Halfway to Achieving Your Goal!",
                $"You’re halfway through your goal. Keep pushing!\nGoal Description: {goalInfo.Description}",
                $"You’re halfway through your goal. Keep pushing!\nGoal Description: {goalInfo.Description}"
            ), halfwayPoint);

        // Schedule the one day left reminder email job
        var notifyDate = goalInfo.endGoalDate.AddDays(-1);
        var jobId = BackgroundJob.Schedule(() =>
            _sendGridEmailService.SendEmail(
                userInfo.Email,
                "One Day to Go!",
                $"You're almost there! Just one day left to reach your goal. Stay focused!\nGoal Description: {goalInfo.Description}",
                $"You're almost there! Just one day left to reach your goal. Stay focused!\nGoal Description: {goalInfo.Description}"
            ), notifyDate);

        // Update the goal with the scheduled Hangfire job IDs
        _goalService.UpdateGoalHangfireJobIds(createdGoalId, halfwayJobId, jobId);

        // Return success response
        return Ok(new
        {
            message = "Goal created successfully!"
        });
    }
    catch (Exception ex)
    {
        // Log the error and return a 500 Internal Server Error
        _logger.LogError($"Error occurred while creating the goal: {ex.Message}");
        return StatusCode(500, "Internal server error while processing your request.");
    }
}


        [HttpPost("UpdateGoal")]
        public IActionResult UpdateGoal([FromBody] GoalViewModel goalInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (goalInfo.Description == null)
            {
                return BadRequest("Goal description is required.");
            }

            // Fetch the existing goal from the database
            var existingGoal = _goalService.GetGoals(goalInfo.userSecretId).FirstOrDefault(g => g.GoalId == goalInfo.GoalId);

            if (existingGoal == null)
            {
                return NotFound("Goal not found.");
            }

            // Delete the old Hangfire jobs if they exist
            if (!string.IsNullOrEmpty(existingGoal.HangfireHalfwayJobId))
            {
                BackgroundJob.Delete(existingGoal.HangfireHalfwayJobId);
            }
            if (!string.IsNullOrEmpty(existingGoal.HangfireJobId))
            {
                BackgroundJob.Delete(existingGoal.HangfireJobId);
            }

            _goalService.UpdateGoal(goalInfo);

            var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userUniqueIdentifier))
            {
                return RedirectToAction("Error", "Home");
            }

            var userInfo = _userService.FindUser(userUniqueIdentifier);
            if (userInfo == null)
            {
                return BadRequest("User not found.");
            }

            var totalDuration = goalInfo.endGoalDate - goalInfo.startingGoalDate;

            var halfwayPoint = goalInfo.startingGoalDate.Add(totalDuration / 2);

            var halfwayJobId = BackgroundJob.Schedule(() =>
                _sendGridEmailService.SendEmail(userInfo.Email,
                "Halfway to Achieving Your Goal!",
                "You�re halfway through your goal. Keep pushing!\n\nGoal Description: " + goalInfo.Description,
                "You�re halfway through your goal. Keep pushing!\n\nGoal Description: " + goalInfo.Description),
                halfwayPoint);


            var notifyDate = goalInfo.endGoalDate.AddDays(-1);
            var newJobId = BackgroundJob.Schedule(() =>
                _sendGridEmailService.SendEmail(userInfo.Email,
                "One Day to Go!",
                "You're almost there! Just one day left to reach your goal. Stay focused!\nGoal Description: " + goalInfo.Description,
                "You're almost there! Just one day left to reach your goal. Stay focused!\nGoal Description: " + goalInfo.Description),
                notifyDate);

            _goalService.UpdateGoalHangfireJobIds(goalInfo.GoalId, halfwayJobId, newJobId);

            return Ok(new
            {
                message = "Goal updated successfully!"
                // TODO: Redirect to "Goals"
            });
        }

        [HttpPost("ResolveGoal")]
        public IActionResult ResolveGoal([FromBody] GoalViewModel goalInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (goalInfo.GoalId == 0)
            {
                return BadRequest("Goal information is null.");
            }

            var goalToUpdate = _goalService.GetGoals(goalInfo.userSecretId).FirstOrDefault(g => g.GoalId == goalInfo.GoalId);

            if (goalToUpdate == null)
            {
                return NotFound("Goal not found.");
            }

            if (!string.IsNullOrEmpty(goalToUpdate.HangfireHalfwayJobId))
            {
                BackgroundJob.Delete(goalToUpdate.HangfireHalfwayJobId);
            }
            if (!string.IsNullOrEmpty(goalToUpdate.HangfireJobId))
            {
                BackgroundJob.Delete(goalToUpdate.HangfireJobId);
            }

            
            goalInfo.resolved = true;
            _goalService.UpdateGoal(goalInfo);

            return Ok(new
            {
                message = "Goal updated successfully! Awesome job!"
                // TODO: Redirect to "Goals"
            });
        }
    }
}
