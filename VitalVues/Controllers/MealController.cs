using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using VVData.Data;
using Hangfire;
using System;
using Services.Services;
using System.Collections.Generic;
using VitalVues;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/MealController")]
public class MealsController : Controller
{
    private readonly IMealService _mealService;
    private readonly IUserService _userService;


    public MealsController(IMealService mealService, IUserService userService)
    {
        _mealService = mealService;
        _userService = userService;
    }

    [HttpGet("Meals")]
    public IActionResult Meals()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("SignIn", "Account");
        }

        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var user = _userService.FindUser(userUniqueIdentifier);

        if (user.SubscriptionEndDate == null || user.SubscriptionEndDate <= DateTime.Now.Date)
        {
            return RedirectToAction("PaymentRequired", "Home");
        }

        return View(); 
    }

    // Handles form submission to create a new meal
    [HttpPost("CreateMeal")]
    public IActionResult CreateMeal([FromBody] MealViewModel mealViewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        mealViewModel.userID = userUniqueIdentifier;

        _mealService.CreateMeal(mealViewModel);

        return Ok(new
        {
            message = "Meal created successfully!"
        });
    }


    //retrieve meals by user
    [HttpGet("GetMeals")]
    public IActionResult GetMeals(string userId)
    {
        var meals = _mealService.GetMealsByUser(userId);

        if (meals == null || !meals.Any())
        {
            return NotFound(new { message = "No meals found for this user." });
        }

        return Ok(meals);
    }

    [HttpPost("DeleteMeal")]
    public IActionResult DeleteMeal([FromBody] int mealId)
    {
        _mealService.DeleteMeal(mealId);
        return Ok(new { message = "Meal deleted successfully!" });
    }


    [HttpPost("UpdateMeal")]
    public IActionResult UpdateMeal([FromBody] MealViewModel mealViewModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _mealService.UpdateMeal(mealViewModel);
        return Ok(new { message = "Meal updated successfully!" });
    }




}