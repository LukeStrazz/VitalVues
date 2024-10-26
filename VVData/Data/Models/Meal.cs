using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVData.Data.Models;
    public class Meal: TrackableEntry
    {
    public string UserID { get; set; }
    public string MealName { get; set; } 

    public string DayOfWeek { get; set; } 

    public string MealType { get; set; }

    public string MealDescription { get; set; } 

    public List<string> Ingredients { get; set; } = new List<string>(); 

    public int PrepTime { get; set; } 
}
