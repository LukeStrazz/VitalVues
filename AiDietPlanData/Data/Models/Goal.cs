using Data;
using System;
namespace AiDietPlanData.Data.Models;

public class Goal : Entity
{
    public DateTime startingGoalDate { get; set; }
    public DateTime endGoalDate { get; set; }
    public required string Description { get; set; }
}

