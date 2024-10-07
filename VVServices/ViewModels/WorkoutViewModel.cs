using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Services.ViewModels;

public class WorkoutViewModel
{
  public int WorkoutId { get; set; }
  public required string userSecretId { get; set; }
  public string? Type { get; set; }
  public string? SubType { get; set; }
  public int Set { get; set; }
  public int Rep { get; set; }
  public required string Day { set; get; }
  public bool resolved { get; set; }
  public int Duration { get; set; }


}
