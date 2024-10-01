using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using VitalVues.Models;

namespace VitalVues.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

            // Enqueue a background job
            BackgroundJob.Enqueue(() => LogMessage("Hello world from Hangfire!"));

            //Schedule a background job
            BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromDays(7));

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Job method to be called by Hangfire
        public static void LogMessage(string message)
        {
            // Log the message or perform any other task
            Console.WriteLine(message); 
        }
    }
}
