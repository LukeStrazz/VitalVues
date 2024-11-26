using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IBMIService
{
    Task<List<BMIProgress>> GetAllBMIProgressAsync(string userId);
    Task<BMIProgress> AddBMIProgressAsync(string userId, double bmi);
    Task<bool> DeleteBMIRecordAsync(int id, string userId);
}
