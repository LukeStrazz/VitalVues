using AiDietPlanData.Data;
using AiDietPlanData.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> UserExists(string uniqueToken)
    {
		return await _context.People.AnyAsync(p => p.Sid == uniqueToken);
	}

    public UserInfoViewModel FindUser(string uniqueToken)
    {

        var user = _context.People.FirstOrDefault(p => p.Sid == uniqueToken);

        var userViewModel = new UserInfoViewModel
        {
            Id = user.Id,
            Sid = user.Sid,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            CurrentWeight = user.CurrentWeight,  
            StartingWeight = user.StartingWeight,
            Allergies = user.Allergies,
            Age = user.Age,
            Birthday = user.Birthday,
            ProfileImage = user.ProfileImage
        };

        return userViewModel;
    }

	public async Task CreateUser(UserInfoViewModel info)
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
                Sid = info.Sid,
                ProfileImage = info.ProfileImage,
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

    public void UpdateUser(UserInfoViewModel info)
    {
        if (info != null)
        {
            var user = _context.People.FirstOrDefault(p => p.Sid == info.Sid);

            var today = DateTime.Today;
            var age = today.Year - info.Birthday.Year;

            if (info.Birthday.Date > today.AddYears(-age))
            {
                age--;
            }

            user.FirstName = info.FirstName;
            user.LastName = info.LastName;
            user.Username = info.Username;
            user.Email = info.Email;
            user.Age = age;
            user.Birthday = info.Birthday;
            user.StartingWeight = info.StartingWeight;
            user.CurrentWeight = info.StartingWeight;
            user.Allergies = info.Allergies;
            user.ProfileImage = info.ProfileImage;
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
