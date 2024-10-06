using Services.ViewModels;

namespace Services.Interfaces;

public interface IWorkoutService
{
  public IEnumerable<WorkoutViewModel> GetWorkouts(string userSecretId);
  public void CreateWorkout(WorkoutViewModel workoutViewModel);
  public void UpdateWorkout(WorkoutViewModel workoutViewModel);
  public void ResolveWorkout(int WorkoutId);
}