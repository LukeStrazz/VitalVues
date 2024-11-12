using Services.Interfaces;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data.Models;
using VVData.Data;

namespace Services.Services;

public class MealService : IMealService
{
    private readonly DatabaseContext _context;

    public MealService(DatabaseContext context)
    {
        _context = context;
    }

    public void CreateMeal(MealViewModel mealViewModel)
    {
        var meal = new Meal
        {
            UserID = mealViewModel.userID,
            MealName = mealViewModel.MealName,
            DayOfWeek = mealViewModel.DayOfWeek,
            MealType = mealViewModel.MealType,
            MealDescription = mealViewModel.MealDescription,
            Ingredients = mealViewModel.Ingredients,
            PrepTime = mealViewModel.PrepTime
        };

        _context.Meals.Add(meal);
        _context.SaveChanges();
    }

    public IEnumerable<MealViewModel> GetMealsByUser(string userId)
    {
        var meals = _context.Meals
            .Where(m => m.UserID == userId)
            .Select(m => new MealViewModel
            {
                userID = m.UserID,
                MealPlanId = m.Id,
                MealName = m.MealName,
                DayOfWeek = m.DayOfWeek,
                MealType = m.MealType,
                MealDescription = m.MealDescription,
                Ingredients = m.Ingredients,
                PrepTime = m.PrepTime
            })
            .ToList();

        return meals;
    }


    public void DeleteMeal(int mealId)
    {
        var meal = _context.Meals.Find(mealId);
        if (meal != null)
        {
            _context.Meals.Remove(meal);
            _context.SaveChanges();
        }
    }
    public void UpdateMeal(MealViewModel mealViewModel)
    {
        var meal = _context.Meals.Find(mealViewModel.MealPlanId);
        if (meal != null)
        {
            meal.MealName = mealViewModel.MealName;
            meal.DayOfWeek = mealViewModel.DayOfWeek;
            meal.MealType = mealViewModel.MealType;
            meal.MealDescription = mealViewModel.MealDescription;
            meal.Ingredients = mealViewModel.Ingredients; 
            meal.PrepTime = mealViewModel.PrepTime;

            _context.SaveChanges(); 
        }
    }

}
