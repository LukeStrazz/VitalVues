using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class GoalViewModel
{
    public int GoalId { get; set; }
    public string userSecretId { get; set; }
    public  string title { set; get; }
    public bool resolved { get; set; }
    public DateTime startingGoalDate { get; set; }
    public DateTime endGoalDate { get; set; }
    public double? targetWeight { get; set; }
    public required string Description { get; set; }
    public string? HangfireJobId { get; set; }
    public string? HangfireHalfwayJobId { get; set; }  
}
