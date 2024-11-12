using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VitalVues.Models;
using Data.Data.Models;
using Microsoft.AspNetCore.Mvc;
using VVData.Data;

namespace VitalVues.Controllers
{
    public class BMIController : Controller
    {
        private readonly DatabaseContext _context;

        public BMIController(DatabaseContext context)
        {
            _context = context;
        }

     
        [HttpPost]
        public async Task<IActionResult> CalculateBMIImperial(double weightImperial, int feetImperial, int inchesImperial)
        {
            double heightInInches = (feetImperial * 12) + inchesImperial;
            double bmi = (weightImperial / (heightInInches * heightInInches)) * 703;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newRecord = new BMIProgress
            {
                UserId = userId,
                BMIValue = Math.Round(bmi, 2),
                RecordedAt = DateTime.Now
            };
            _context.BMIProgress.Add(newRecord);
            await _context.SaveChangesAsync();

            var bmiProgress = await _context.BMIProgress
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.RecordedAt)
                .ToListAsync();

            return Json(new
            {
                success = true,
                bmi = Math.Round(bmi, 2),
                bmiProgress = bmiProgress.Select(b => new
                {
                    Id = b.Id,
                    RecordedAt = b.RecordedAt.ToString("MM/dd/yyyy HH:mm"),
                    BMIValue = b.BMIValue
                }).ToList()
            });
        }

       
        [HttpPost]
        public async Task<IActionResult> CalculateBMIMetric(double weightMetric, double heightMetric)
        {
            double heightInMeters = heightMetric / 100;
            double bmi = weightMetric / (heightInMeters * heightInMeters);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newRecord = new BMIProgress
            {
                UserId = userId,
                BMIValue = Math.Round(bmi, 2),
                RecordedAt = DateTime.Now
            };
            _context.BMIProgress.Add(newRecord);
            await _context.SaveChangesAsync();

            var bmiProgress = await _context.BMIProgress
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.RecordedAt)
                .ToListAsync();

            return Json(new
            {
                success = true,
                bmi = Math.Round(bmi, 2),
                bmiProgress = bmiProgress.Select(b => new
                {
                    Id = b.Id,
                    RecordedAt = b.RecordedAt.ToString("MM/dd/yyyy HH:mm"),
                    BMIValue = b.BMIValue
                }).ToList()
            });
        }

        
        [HttpPost]
        public async Task<IActionResult> DeleteBMIRecord(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var record = await _context.BMIProgress.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (record == null)
            {
                return Json(new { success = false, message = "BMI record not found." });
            }

            _context.BMIProgress.Remove(record);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBMIProgress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bmiProgress = await _context.BMIProgress
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.RecordedAt)
                .Select(b => new
                {
                    Id = b.Id,
                    RecordedAt = b.RecordedAt.ToString("MM/dd/yyyy HH:mm"),
                    BMIValue = b.BMIValue
                })
                .ToListAsync();

            return Json(new { bmiProgress });
        }

    }
}






























