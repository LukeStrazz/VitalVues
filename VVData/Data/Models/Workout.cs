using VVData.Data;
using System;
namespace VVData.Data.Models;

public class Workout : TrackableEntry
{
  public string UserID { get; set; }
  public string? Type { get; set; }
  public string? SubType { get; set; }
  public int Set { get; set; }
  public int Rep { get; set; }
  public required string Day { set; get; }
  public bool resolved { get; set; }
  public int Duration { get; set; }

}
