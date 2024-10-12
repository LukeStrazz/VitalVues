using Microsoft.AspNetCore.Mvc;

namespace VitalVues.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult About()
        {
            return View();
        }


        public IActionResult Contact()
        {
            return View();
        }
    }
}
