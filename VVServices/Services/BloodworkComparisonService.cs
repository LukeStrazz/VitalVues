using Data.Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VVData.Data;

namespace Services.Services
{
    public class BloodworkComparisonService : IBloodworkComparisonService
    {
        private readonly DatabaseContext _context;

        public BloodworkComparisonService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<BloodworkComparisonViewModel>> GetBloodworkComparisonAsync(string userId)
        {
            // Fetch the two most recent bloodwork entries for the user
            var bloodworks = await _context.Bloodworks
                .Include(b => b.BloodTests)
                    .ThenInclude(bt => bt.Test) // Include Test details
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .Take(2)
                .ToListAsync();

            if (bloodworks.Count < 2)
            {
                return new List<BloodworkComparisonViewModel>();
            }

            var previousBloodwork = bloodworks[1];
            var currentBloodwork = bloodworks[0];

            var comparisonList = new List<BloodworkComparisonViewModel>();

            foreach (var previousTest in previousBloodwork.BloodTests.SelectMany(bt => bt.Test))
            {
                var currentTest = currentBloodwork.BloodTests
                    .SelectMany(bt => bt.Test)
                    .FirstOrDefault(t => t.TestName == previousTest.TestName);

                if (currentTest != null)
                {
                    comparisonList.Add(new BloodworkComparisonViewModel
                    {
                        TestName = previousTest.TestName,
                        PreviousValue = previousTest.Result,
                        CurrentValue = currentTest.Result
                    });
                }
            }

            return comparisonList;
        }
    }
}

