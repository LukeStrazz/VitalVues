using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;

namespace VitalVues.Controllers
{
    public class FastingController : Controller
    {
       
        public IActionResult Fasting()
        {
            var model = new FastingViewModel
            {
                Hours = 0,
                Minutes = 0,
                Seconds = 0
               
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Fasting(FastingViewModel model)
        {
            
            return View(model);
        }
    }
}
