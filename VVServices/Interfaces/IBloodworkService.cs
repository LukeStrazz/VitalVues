using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data.Models;

namespace Services.Interfaces;

public interface IBloodworkService
{
    public void AddBloodwork(IEnumerable<TestResultViewModel> result, string userId, DateTime bloodworkDate);
    public List<BloodworkViewModel> GetBloodworks(string userUniqueIdentifier);
    public void UpdateBloodwork(IEnumerable<TestResultViewModel> result);
}
