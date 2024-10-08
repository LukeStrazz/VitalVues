﻿using VVData.Data;
using System;
namespace VVData.Data.Models;

public class Goal : TrackableEntry
{
    public string UserID { get; set; }
    public bool resolved { get; set; }
    public double? targetWeight { get; set; }
    public DateTime startingGoalDate { get; set; }
    public DateTime endGoalDate { get; set; }
    public required string Description { get; set; }
}

