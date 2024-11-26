using Data.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBloodworkComparisonService
    {
        Task<List<BloodworkComparisonViewModel>> GetBloodworkComparisonAsync(string userId);
    }
}
