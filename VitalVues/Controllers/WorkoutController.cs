using Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;
using System.Collections.Generic;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/WorkoutController")]
public class WorkoutController : Controller
{
    private readonly IWorkoutService _workoutService;
    private readonly IUserService _userService;


    public WorkoutController(IWorkoutService workoutService, IUserService userService)
    {
        _workoutService = workoutService;
        _userService = userService;
    }

    [NoCacheHeaders]
    [HttpGet("Workout")]
    public ActionResult Workout()
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

        var workouts = _workoutService.GetWorkouts(userUniqueIdentifier).ToList();

        var userInfo = new UserInfoViewModel
        {
            Sid = userUniqueIdentifier,
            Workouts = workouts
        };

        return View(userInfo);
    }

    [HttpPost("CreateWorkout")]
    public ActionResult CreateWorkout([FromBody] WorkoutViewModel workoutInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (workoutInfo.userSecretId == null)
        {
            return BadRequest("Workout information is null.");
        }

        _workoutService.CreateWorkout(workoutInfo);

        return Ok(new
        {
            message = "Workout created successfully!"
        });
    }



    [HttpPost("ResolveWorkout")]
    public ActionResult ResolveWorkout([FromBody] WorkoutViewModel workoutInfo)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (workoutInfo.WorkoutId == null)
        {
            return BadRequest("Workout information is null.");
        }

        _workoutService.ResolveWorkout(workoutInfo.WorkoutId);

        return Ok(new
        {
            message = "Workout deleted successfully!"
        });
    }
}
