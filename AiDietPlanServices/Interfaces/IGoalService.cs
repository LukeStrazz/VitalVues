using AiDietPlanData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IGoalService
{
    public IEnumerable<GoalViewModel> GetGoals(string userSecretId);
    public void CreateGoal(GoalViewModel goalViewModel);
    public void UpdateGoal(GoalViewModel goalViewModel);
    public void ResolveGoal(int goalId);
}
