using Data.Data.Enums;
using Data.Data.Models;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;
using VVData.Data.Models;

namespace Services.Services;

public class BloodworkService : IBloodworkService
{
    private readonly DatabaseContext _databaseContext;
    public BloodworkService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public void AddBloodwork(IEnumerable<TestResultViewModel> testResult, string userId, DateTime bloodworkDate)
    {
        if (testResult == null) throw new ArgumentNullException(nameof(testResult));

        List<Test> testList = new List<Test>();
        BloodTest bloodTest = new BloodTest();
        Bloodwork bloodWork = new Bloodwork();
        bloodWork.BloodTests = new List<BloodTest>();

        foreach (TestResultViewModel resultViewModel in testResult)
        {
            Test test = new Test();

            test.TestName = resultViewModel.TestName;
            test.Result = resultViewModel.Result;

            GradeEnum grade;

            if (string.IsNullOrEmpty(resultViewModel.Grade))
            {
                grade = GradeEnum.Medium;
            }
            else if (resultViewModel.Grade.StartsWith("H"))
            {
                grade = GradeEnum.High;
            }
            else if (resultViewModel.Grade.StartsWith("L"))
            {
                grade = GradeEnum.Low;
            }
            else
            {
                grade = GradeEnum.Medium;
            }

            test.Grade = grade;

            testList.Add(test);
            _databaseContext.Add(test); 
        }

        _databaseContext.Add(bloodTest);

        bloodTest.Test = testList;
        bloodTest.BloodworkDate = bloodworkDate;
        bloodTest.SubmissionDate = DateTime.Now;

        bloodWork.BloodTests.Add(bloodTest);

        var user = _databaseContext.People.Where(p => p.Sid == userId).FirstOrDefault();
        if (user == null)
        {
            return;
        }

        if (user.BloodWorks == null)
        {
            user.BloodWorks = new List<Bloodwork>();
        }
        bloodWork.UserId = user.Sid;
        user.BloodWorks.Add(bloodWork);

        _databaseContext.SaveChanges();
    }


    public void UpdateBloodwork(IEnumerable<TestResultViewModel> result)
    {
        throw new NotImplementedException();
    }
}
