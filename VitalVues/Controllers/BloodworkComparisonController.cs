﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Data.Models;
using Data.Data;  // Ensure the namespace aligns with your project structure
using VVData.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Data.Models;
using VVData.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace VitalVues.Controllers
{
    public class BloodworkComparisonController : Controller
    {
        private readonly DatabaseContext _context;

        public BloodworkComparisonController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Bloodwork Comparison View
        public async Task<IActionResult> Comparison()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the two most recent bloodwork entries for the user
            var bloodworks = await _context.Bloodworks
                .Include(b => b.BloodTests)
                    .ThenInclude(bt => bt.Test)  // Ensure BloodTests include individual Test details
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedDate)
                .Take(2)
                .ToListAsync();

            if (bloodworks.Count < 2)
            {
                return View(new List<BloodworkComparisonViewModel>());
            }

            var previousBloodwork = bloodworks[1];
            var currentBloodwork = bloodworks[0];

            // Prepare the comparison view model
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

            return View(comparisonList);
        }
    }
}


