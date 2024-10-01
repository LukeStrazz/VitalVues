using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace Data.Data.Models;

public class SemanticMemoryDto : TrackableEntry
{
    public string UserSid { get; set; }
    public string MemoryType { get; set; }
    public string Content { get; set; }
}
