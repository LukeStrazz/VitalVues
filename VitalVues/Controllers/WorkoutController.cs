using Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;
using System.Collections.Generic;

namespace VitalVues.Controllers
{
  [ApiController]
  [Route("api/WorkoutController")]
  public class WorkoutController : Controller
  {
    private readonly IWorkoutService _workoutService;

    public WorkoutController(IWorkoutService workoutService)
    {
      _workoutService = workoutService;
    }


    [HttpGet("Workout")]
    public ActionResult Workout()
    {
      var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

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

      if (workoutInfo.userSecretId == null)
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
}
