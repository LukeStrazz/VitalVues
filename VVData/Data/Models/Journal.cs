using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace Data.Data.Models;

public class Journal : TrackableEntry
{
    public string UserID { get; set; }
    public DateTime JournalDate { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public List<Bloodtest>? BloodtestOfThisWeek { get; set; }
    public List<Workout>? WorkoutsOfThisWeek { get; set; }
    public List<Goal>? GoalsInProgress { get; set; }
    public List<Chat>? ChatsThisWeek { get; set; }
}
