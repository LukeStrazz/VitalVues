using System;
using VVData.Data;
using VVData.Data.Models;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.ViewModels;

namespace Services.Services;

public class GoalService : IGoalService
{
    private readonly ILogger<GoalService> _logger;
    private readonly DatabaseContext _context;
    public GoalService(ILogger<GoalService> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    IEnumerable<GoalViewModel> IGoalService.GetGoals(string userSecretId)
    {
        var user = _context.People.FirstOrDefault(g => g.Sid == userSecretId);

        var goals = _context.Goals.Where(g => g.UserID == userSecretId);

        if(goals == null)
        {
            return Enumerable.Empty<GoalViewModel>();
        }

        var viewModelGoals = new List<GoalViewModel>();
        foreach( var goal in goals)
        {
            var goalToAdd = new GoalViewModel { 
                Description = goal.Description,
                startingGoalDate = goal.startingGoalDate,
                endGoalDate = goal.endGoalDate,
                resolved = goal.resolved,
                userSecretId = goal.UserID,
                GoalId = goal.Id
            };

            viewModelGoals.Add(goalToAdd);
        }

        return viewModelGoals;
    }

    public void CreateGoal(GoalViewModel goalViewModel)
    {
        var newGoal = new Goal
        {
            Description = goalViewModel.Description,
            startingGoalDate = goalViewModel.startingGoalDate,
            endGoalDate = goalViewModel.endGoalDate,
            resolved = goalViewModel.resolved,
            UserID = goalViewModel.userSecretId
        };

        _context.Goals.Add(newGoal);
        _context.SaveChanges();
    }


    public void UpdateGoal(GoalViewModel goalViewModel)
    {
        var goalToUpdate = _context.Goals.FirstOrDefault(g => g.Id == goalViewModel.GoalId);

        if (goalToUpdate != null)
        {
            goalToUpdate.Description = goalViewModel.Description;
            goalToUpdate.startingGoalDate = goalViewModel.startingGoalDate;
            goalToUpdate.endGoalDate = goalViewModel.endGoalDate;
            goalToUpdate.resolved = goalViewModel.resolved;

            _context.SaveChanges();
        }
        else
        {
            throw new InvalidOperationException("Goal not found");
        }
    }

    public void ResolveGoal(int goalId)
    {
        var goalToUpdate = _context.Goals.FirstOrDefault(g => g.Id == goalId);

        goalToUpdate.resolved = true;

        _context.SaveChanges();
    }
}

