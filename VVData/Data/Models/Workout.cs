using VVData.Data;
using System;
namespace VVData.Data.Models;

public class Workout : TrackableEntry
{
  public string UserID { get; set; }
  public required string Type { get; set; }
  public required string SubType { get; set; }
  public int Set { get; set; }
  public int Rep { get; set; }
  public required DayOfWeek Day { set; get; }
  public bool resolved { get; set; }

}
