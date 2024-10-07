using Data.Data.Enums;
using Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;
using VVData.Data.Models;

namespace Services.ViewModels;

public class BloodworkViewModel
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public virtual List<BloodTestViewModel> BloodTests { get; set; }
}

public class BloodTestViewModel
{
    public int Id { get; set; }
    public DateTime SubmissionDate { get; set; }
    public DateTime BloodworkDate { get; set; }
    public List<TestViewModel> Test { get; set; }
}

public class TestViewModel
{
    public int Id { get; set; }
    public string TestName { get; set; }
    public string Result { get; set; }
    public GradeEnum Grade { get; set; }
}