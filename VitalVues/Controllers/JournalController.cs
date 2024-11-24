using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.ViewModels;
using VVData.Data;
using Data.Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.Services;

namespace VitalVues.Controllers;

    [ApiController]
    [Route("api/JournalController")]
    public class JournalsController : Controller
    {
    
        private readonly IJournalService _journalService;
        private readonly DatabaseContext _context;
        private readonly IChatService _chatService;
        private readonly IBloodworkService _bloodworkService;
        private readonly IGoalService _goalService;
        private readonly IWorkoutService _workoutService;
        private readonly IUserService _userService;

        public JournalsController(IJournalService journalService, DatabaseContext context, IChatService chatService, IBloodworkService bloodworkService, IGoalService goalService, IWorkoutService workoutService, IUserService userService)
        {
            _journalService = journalService;
            _chatService = chatService;
            _bloodworkService = bloodworkService;
            _goalService = goalService;
            _context = context;
            _workoutService = workoutService;
            _userService = userService;
        }

        [NoCacheHeaders]
        [HttpGet("Journal")]
        public IActionResult Journal()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignIn", "Account");
            }

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

            if (string.IsNullOrEmpty(userUniqueIdentifier))
            {
                return RedirectToAction("Error", "Home");
            }

            var journals = _journalService.GetJournals(userUniqueIdentifier).ToList();
            var chats = _chatService.GetChats(userUniqueIdentifier).ToList();
            var bloodworks = _bloodworkService.GetBloodworks(userUniqueIdentifier).ToList();
            var goals = _goalService.GetGoals(userUniqueIdentifier).ToList();
            var workouts = _workoutService.GetWorkouts(userUniqueIdentifier).ToList();

            var userInfo = new UserInfoViewModel
            {   
                Sid = userUniqueIdentifier,
                Journals = journals,
                Goals = goals,
                Chats = chats,
                Bloodworks = bloodworks,
                Workouts = workouts
            };

            return View(userInfo);
        }

        
        [HttpPost("CreateJournal")]
        public IActionResult CreateJournal([FromBody] JournalViewModel journalViewModel)
        {
                if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (journalViewModel.Content == null)
            {
                return BadRequest("Journal content is null or empty.");
            }

            _journalService.CreateJournal(journalViewModel);

            return Ok(new
            {
                message = "Journal created successfully!"
            });
        }



        [HttpPost("DeleteJournal")]
        public IActionResult DeleteJournal(int id)
        {
            if(!ModelState.IsValid)
            {
            return BadRequest(ModelState);
            }
            
            _journalService.DeleteJournal(id);

        return Ok(new
        {
            message = "Journal has been deleted!"
        });
        }


        [HttpGet("GetJournalDetails")]
         public IActionResult GetJournalDetails(int journalId)
        {
  
        var journal = _context.Journals
                              .Include(j => j.Workouts)
                              .Include(j => j.BloodTests)
                              .Include(j => j.Goals)
                              .FirstOrDefault(j => j.Id == journalId);

        if (journal == null)
        {
            return NotFound();  
        }

        
        var response = new
        {
            content = journal.Content,
            title = journal.Title,
            journalDate = journal.JournalDate.ToString("MM/dd/yyyy"),
            workouts = journal.Workouts.Select(w => new { type = w.SubType, date = w.Day }).ToList(),
            bloodTests = journal.BloodTests.Select(bt => new { date = bt.CreatedDate.ToString("MM/dd/yyyy") }).ToList(),
            goals = journal.Goals.Select(g => new { targetWeight = g.targetWeight, endDate = g.endGoalDate.ToString("MM/dd/yyyy") }).ToList()
        };

        return Json(response);  
    }


        

        [HttpPost("EditJournal")]
        public IActionResult EditJournal(Journal journal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            _journalService.UpdateJournal(journal);

            return Ok(journal);
        }


        

    }

