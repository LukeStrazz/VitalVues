﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace Data.Data.Models;

public class Chat : TrackableEntry
{
    public string UserSID { get; set; }
    public DateTime ChatDate { get; set; }
    public string ChatTopic { get; set; }
    public virtual required List<Message> Messages { get; set; }
    public List<Journal> Journals { get; set; } = new List<Journal>();
}
