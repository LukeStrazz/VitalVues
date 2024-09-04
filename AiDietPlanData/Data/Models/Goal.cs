using Data.Data;
using System;
namespace AiDietPlanData.Data.Models;

public class Goal : TrackableEntry
{
    public string UserID { get; set; }
    public bool resolved { get; set; }
    public DateTime startingGoalDate { get; set; }
    public DateTime endGoalDate { get; set; }
    public required string Description { get; set; }
}

