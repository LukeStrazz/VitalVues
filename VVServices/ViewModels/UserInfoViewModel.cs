using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class UserInfoViewModel
{
    public int Id { get; set; }

    public string Sid { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public string ProfileImage { get; set; }

    public float Age { get; set; }
    public DateTime Birthday { get; set; }
    public double StartingWeight { get; set; }
    public double CurrentWeight { get; set; }
    public List<string>? Allergies { get; set; }
    public List<GoalViewModel>? Goals { get; set; }
    public List<WorkoutViewModel>? Workouts { get; set; }
    public List<ChatViewModel>? Chats { get; set; }
}

