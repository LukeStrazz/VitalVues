using System;
namespace AiDietPlanData.Data.Models;

public class Person
{
    public required string Name { get; set; }
    public required float Age { get; set; }
    public required DateTime Birthday { get; set; }
    public double StartingWeight { get; set; }
    public double CurrentWeight { get; set; }
    public List<string>? Allergies { get; set; }
    public List<Goal>? Goals { get; set; }
}
