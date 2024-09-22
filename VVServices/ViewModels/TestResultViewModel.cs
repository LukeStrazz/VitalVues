using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels;

public class TestResultViewModel
{
    public DateTime TestDate { get; set; }
    public string TestName { get; set; }
    public string Result { get; set; }
    public string Grade { get; set; }
}
