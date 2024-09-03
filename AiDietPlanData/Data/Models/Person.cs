using Data.Data;
using System;
namespace AiDietPlanData.Data.Models;

public class Person : TrackableEntry
{
	public required string Sid { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required float Age { get; set; }
    public required DateTime Birthday { get; set; }
    public double StartingWeight { get; set; }
    public double CurrentWeight { get; set; }
    public List<string>? Allergies { get; set; }
    public List<Goal>? Goals { get; set; }
}
