using Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;

namespace VitalVues.Controllers;


[ApiController]
[Route("api/GoalController")]
public class GoalController : Controller
{
    private readonly IGoalService _goalService;

    public GoalController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpGet("Goal")]
    public IActionResult Goal()
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        var goals = _goalService.GetGoals(userUniqueIdentifier);

        return View(goals);
    }

    [HttpPost("CreateGoal")]
    public IActionResult CreateGoal([FromBody] GoalViewModel goalInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (goalInfo.Description == null)
        {
            return BadRequest("Goal information is null.");
        }

        _goalService.CreateGoal(goalInfo);

        return Ok(new
        {
            message = "Goal created successfully!"
            // TODO: Redirect to "Goals"
        });
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
            return BadRequest("Goal information is null.");
        }

        _goalService.UpdateGoal(goalInfo);

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

        if (goalInfo.GoalId == null)
        {
            return BadRequest("Goal information is null.");
        }

        _goalService.ResolveGoal(goalInfo.GoalId);

        return Ok(new
        {
            message = "Goal updated successfully! Awesome job!"
            // TODO: Redirect to "Goals"
        });
    }
}