using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface IMealService
{
    public void CreateMeal(MealViewModel mealViewModel);
    public IEnumerable<MealViewModel> GetMealsByUser(string userId); // Add this
    public void DeleteMeal(int mealId);
    public void UpdateMeal(MealViewModel mealViewModel);

}