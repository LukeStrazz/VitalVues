using System;
using VVData.Data;
using VVData.Data.Models;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services;

public class WorkoutService : IWorkoutService
{

  private readonly ILogger<WorkoutService> _logger;
  private readonly DatabaseContext _context;

  public WorkoutService(ILogger<WorkoutService> logger, DatabaseContext context)
  {
    _logger = logger;
    _context = context;
  }

  IEnumerable<WorkoutViewModel> IWorkoutService.GetWorkouts(string userSecretId)
  {
    var user = _context.People.FirstOrDefault(g => g.Sid == userSecretId);

    var workouts = _context.Workouts.Where(g => g.UserID == userSecretId);


    if (workouts == null)
    {
      return Enumerable.Empty<WorkoutViewModel>();
    }

    var workoutViewModel = new List<WorkoutViewModel>();

    foreach (var workout in workouts)
    {
      var workoutToAdd = new WorkoutViewModel
      {
        WorkoutId = workout.Id,
        userSecretId = workout.UserID,
        Type = workout.Type,
        SubType = workout.SubType,
        Set = workout.Set,
        Rep = workout.Rep,
        Day = workout.Day,
        resolved = workout.resolved
      };

      workoutViewModel.Add(workoutToAdd);
    }
    return workoutViewModel;
  }



  public void CreateWorkout(WorkoutViewModel workoutViewModel)
  {
    var newWorkout = new Workout
    {
      UserID = workoutViewModel.userSecretId,
      Type = workoutViewModel.Type,
      SubType = workoutViewModel.SubType,
      Set = workoutViewModel.Set,
      Rep = workoutViewModel.Rep,
      Day = workoutViewModel.Day,
      resolved = workoutViewModel.resolved
    };
    _context.Workouts.Add(newWorkout);
    _context.SaveChanges();
  }



  public void ResolveWorkout(int WorkoutId)
  {
    var workoutToResolve = _context.Workouts.FirstOrDefault(g => g.Id == WorkoutId);

    workoutToResolve.resolved = true;

    _context.SaveChanges();
  }

  public void UpdateWorkout(WorkoutViewModel workoutViewModel)
  {
    throw new NotImplementedException();
  }
}