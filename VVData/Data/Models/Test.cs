using Data.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace Data.Data.Models;

public class Test : TrackableEntry
{
    public string TestName { get; set; }
    public string Result { get; set; }
    public GradeEnum Grade { get; set; }
}
