using Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AiDietPlan.Controllers;


public class GoalController : Controller {

private readonly IGoalService _goalService;

public GoalController (IGoalService goalService){

  _goalService = goalService;

}


public IActionResult Goal()
    {
        return View();
    }



}