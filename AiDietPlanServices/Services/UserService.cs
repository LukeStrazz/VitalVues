using AiDietPlanData.Data;
using AiDietPlanData.Data.Models;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly DatabaseContext _context;
    public UserService(ILogger<UserService> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void createUser(UserInfoViewModel info)
    {
        if (info != null)
        {
            var today = DateTime.Today;
            var age = today.Year - info.Birthday.Year;

            if (info.Birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            var person = new Person
            {
                FirstName = info.FirstName,
                LastName = info.LastName,
                Username = info.Username,
                Email = info.Email,
                Age = age,
                Birthday = info.Birthday,
                StartingWeight = info.StartingWeight,
                CurrentWeight = info.StartingWeight,
                Allergies = info.Allergies
            };
            _context.People.Add(person);
            _context.SaveChanges();
        }
    }

    public void createGoal(GoalViewModel goalInfo, UserInfoViewModel userInfo)
    {
        if (goalInfo != null && userInfo != null)
        {
            var newGoal = new Goal
            {
                Description = goalInfo.Description,
                startingGoalDate = goalInfo.startingGoalDate,
                endGoalDate = goalInfo.endGoalDate
            };

            var user = _context.People.FirstOrDefault(u => u.Id == userInfo.Id);
            if(user != null)
            {
                _context.Goals.Add(newGoal);
                _context.SaveChanges();

                user.Goals.Add(newGoal);
                _context.SaveChanges();
            }
        }
    }
}
