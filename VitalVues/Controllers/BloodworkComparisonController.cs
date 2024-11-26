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
using Services.Services;
using Services.Interfaces;

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
        private readonly IUserService _userService;

        public BloodworkComparisonController(IBloodworkComparisonService bloodworkComparisonService, IUserService userService)
        {
            _userService = userService;
            _bloodworkComparisonService = bloodworkComparisonService;
        }

        public async Task<IActionResult> Comparison()
        {
            var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(userUniqueIdentifier))
            {
                return RedirectToAction("Error", "Home");
            }

            var user = _userService.FindUser(userUniqueIdentifier);

            if (user.SubscriptionEndDate == null || user.SubscriptionEndDate <= DateTime.Now.Date)
            {
                return RedirectToAction("PaymentRequired", "Home");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comparisonList = await _bloodworkComparisonService.GetBloodworkComparisonAsync(userId);
            return View(comparisonList);
        }
    }
}





