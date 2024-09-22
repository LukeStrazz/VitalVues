using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class FastingViewModel
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public string userID { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }

}
