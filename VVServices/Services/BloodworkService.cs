using Data.Data.Enums;
using Data.Data.Models;
using Microsoft.EntityFrameworkCore;
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

    List<BloodworkViewModel> IBloodworkService.GetBloodworks(string userUniqueIdentifier)
    {
        var bloodworks = _databaseContext.Bloodworks
                            .Where(bw => bw.UserId == userUniqueIdentifier)
                            .Include(bw => bw.BloodTests) // Ensure related BloodTests are loaded
                            .ThenInclude(bt => bt.Test)   // Ensure related Test data is loaded
                            .ToList();

        var bwList = new List<BloodworkViewModel>();

        foreach (var bloodwork in bloodworks)
        {
            var bwvm = new BloodworkViewModel
            {
                Id = bloodwork.Id,
                UserId = bloodwork.UserId,
                BloodTests = bloodwork.BloodTests.Select(bt => new BloodTestViewModel
                {
                    Id = bt.Id,
                    SubmissionDate = bt.SubmissionDate,
                    BloodworkDate = bt.BloodworkDate,
                    Test = bt.Test.Select(t => new TestViewModel
                    {
                        Id = t.Id,
                        TestName = t.TestName,
                        Result = t.Result,
                        Grade = t.Grade
                    }).ToList()
                }).ToList()
            };

            bwList.Add(bwvm);
        }

        return bwList;
    }
}
