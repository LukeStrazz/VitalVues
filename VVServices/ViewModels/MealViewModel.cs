using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class MealViewModel
    {

        public string userID { get; set; }
        public int MealPlanId { get; set; }

        public string MealName { get; set; }

        public string DayOfWeek { get; set; }

        public string MealType { get; set; }

        public string MealDescription { get; set; }

        public List<string> Ingredients { get; set; } 

        public int PrepTime { get; set; } 
    }
}
