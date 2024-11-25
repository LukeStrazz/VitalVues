using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Services.ViewModels;

public class JournalViewModel
{
  
    public int JournalId { get; set; }
    public required string UserID { get; set; }
    public DateTime JournalDate { get; set; } = DateTime.Now;
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool Resolved { get; set; }


    public List<int>? SelectedBloodTestIds { get; set; } = new List<int>();
    public List<int>? SelectedWorkoutIds { get; set; } = new List<int>();
    public List<int>? SelectedGoalIds { get; set; } = new List<int>();
    public List<int>? SelectedChatIds { get; set; } = new List<int>();


    public List<SelectListItem>? BloodTestOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem>? WorkoutOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem>? GoalOptions { get; set; } = new List<SelectListItem>();
    public List<SelectListItem>? ChatOptions { get; set; } = new List<SelectListItem>();
}

public class JournalDetailsViewModel
{   
    public int JournalID { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public float Age { get; set; }
    public string Birthday { get; set; }
    public double StartWeight { get; set; }
    public double CurrWeight { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string JournalDate { get; set; }
    public List<WorkoutViewModel> Workouts { get; set; }
    public List<BloodTestViewModel> BloodTests { get; set; }
    public List<GoalViewModel> Goals { get; set; }
    public List<ChatViewModel> Chats { get; set; }
}



