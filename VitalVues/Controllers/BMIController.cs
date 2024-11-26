using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VitalVues.Controllers
{
    public class BMIController : Controller
    {
        private readonly IBMIService _bmiService;

        public BMIController(IBMIService bmiService)
        {
            _bmiService = bmiService;
        }

        [HttpPost]
        public async Task<IActionResult> CalculateBMIImperial(double weightImperial, int feetImperial, int inchesImperial)
        {
            double heightInInches = (feetImperial * 12) + inchesImperial;
            double bmi = (weightImperial / (heightInInches * heightInInches)) * 703;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newRecord = await _bmiService.AddBMIProgressAsync(userId, bmi);

            var bmiProgress = await _bmiService.GetAllBMIProgressAsync(userId);

            return Json(new
            {
                success = true,
                bmi = newRecord.BMIValue,
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

            var newRecord = await _bmiService.AddBMIProgressAsync(userId, bmi);

            var bmiProgress = await _bmiService.GetAllBMIProgressAsync(userId);

            return Json(new
            {
                success = true,
                bmi = newRecord.BMIValue,
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
            var success = await _bmiService.DeleteBMIRecordAsync(id, userId);

            if (!success)
            {
                return Json(new { success = false, message = "BMI record not found." });
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBMIProgress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bmiProgress = await _bmiService.GetAllBMIProgressAsync(userId);

            return Json(new
            {
                bmiProgress = bmiProgress.Select(b => new
                {
                    Id = b.Id,
                    RecordedAt = b.RecordedAt.ToString("MM/dd/yyyy HH:mm"),
                    BMIValue = b.BMIValue
                }).ToList()
            });
        }
    }
}






























