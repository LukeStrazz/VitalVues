using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;
using VVData.Data.Models;


namespace Data.Data.Models;

public class Journal : TrackableEntry
{
    public required string UserID { get; set; }
    public DateTime JournalDate { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public bool Resolved { get; set; }


    public List<BloodTest> BloodTests { get; set; } = new List<BloodTest>();
    public List<Workout> Workouts { get; set; } = new List<Workout>();
    public List<Goal> Goals { get; set; } = new List<Goal>();
    public List<Chat> Chats { get; set; } = new List<Chat>();
    
}
