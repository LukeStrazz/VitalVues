using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class UserInfoViewModel
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public required float Age { get; set; }
    public required DateTime Birthday { get; set; }
    public double StartingWeight { get; set; }
    public double CurrentWeight { get; set; }
    public List<string>? Allergies { get; set; }
    public List<GoalViewModel>? Goals { get; set; }
}
