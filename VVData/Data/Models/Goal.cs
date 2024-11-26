using VVData.Data;
using System;
using Data.Data.Models;
namespace VVData.Data.Models;

public class Goal : TrackableEntry
{
    public string UserID { get; set; }
     public string title { set; get; }
    public bool resolved { get; set; }
    public double? targetWeight { get; set; }
    public DateTime startingGoalDate { get; set; }
    public DateTime endGoalDate { get; set; }
    public required string Description { get; set; }
    public string? HangfireJobId { get; set; }
    public string? HangfireHalfwayJobId { get; set; }
    public List<Journal> Journals { get; set; } = new List<Journal>();
}

