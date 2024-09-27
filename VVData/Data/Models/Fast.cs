using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace VVData.Data.Models;

public class Fast : TrackableEntry
{

    public string userID { get; set; }
    public DateTime start { get; set; }
    public DateTime end { get; set; }
}
