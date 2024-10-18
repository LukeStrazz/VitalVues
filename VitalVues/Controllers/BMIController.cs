using Microsoft.AspNetCore.Mvc;

namespace VitalVues.Controllers
{
    public class BMIController : Controller
    {
        [HttpPost]
        public IActionResult CalculateBMIImperial(double weightImperial, int feetImperial, int inchesImperial)
        {
            
            double heightInInches = (feetImperial * 12) + inchesImperial;
            double bmi = (weightImperial / (heightInInches * heightInInches)) * 703;

            
            ViewData["BMI"] = Math.Round(bmi, 2);

            
            ViewData["ActiveTab"] = "imperial";

           
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        public IActionResult CalculateBMIMetric(double weightMetric, double heightMetric)
        {
            
            double heightInMeters = heightMetric / 100;
            double bmi = weightMetric / (heightInMeters * heightInMeters);

            
            ViewData["BMI"] = Math.Round(bmi, 2);

            
            ViewData["ActiveTab"] = "metric";

            
            return View("~/Views/Home/Index.cshtml");
        }
    }
}

















