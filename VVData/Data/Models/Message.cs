using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace Data.Data.Models;

public class Message : TrackableEntry
{
    public string role { get; set; }
    public string content { get; set; }
}
