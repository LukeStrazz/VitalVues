using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class JournalViewModel
{
    public string JournalId { get; set; }
    public string UserId { get; set; }
    public DateTime JournalDate { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public List<BloodTestViewModel>? BloodtestOfThisWeek { get; set; }
    public List<WorkoutViewModel>? WorkoutsOfThisWeek { get; set; }
    public List<GoalViewModel>? GoalsInProgress { get; set; }
    public List<ChatViewModel>? ChatsThisWeek { get; set; }
}
