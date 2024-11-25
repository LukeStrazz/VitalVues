using System;
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

using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VitalVues.Controllers
{
    public class BloodworkComparisonController : Controller
    {
        private readonly IBloodworkComparisonService _bloodworkComparisonService;

        public BloodworkComparisonController(IBloodworkComparisonService bloodworkComparisonService)
        {
            _bloodworkComparisonService = bloodworkComparisonService;
        }

        public async Task<IActionResult> Comparison()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comparisonList = await _bloodworkComparisonService.GetBloodworkComparisonAsync(userId);
            return View(comparisonList);
        }
    }
}





