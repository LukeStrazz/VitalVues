using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;
using VVData.Data.Models;

namespace Data.Data.Models;

public class Bloodwork : TrackableEntry
{
    public string UserId { get; set; }
    public virtual List<BloodTest> BloodTests { get; set; }
}
