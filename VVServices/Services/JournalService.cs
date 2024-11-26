using System.Collections.Generic;
using System.Linq;
using VVData.Data.Models;
using Services.ViewModels;
using Services.Interfaces;
using VVData.Data;
using Microsoft.EntityFrameworkCore;
using Data.Data.Models;
using Services.Services;


namespace Services
{
    public class JournalService : IJournalService
    {
        private readonly DatabaseContext _context;
        private readonly IGoalService _goalService;
        private readonly IUserService _userService;
        private readonly IBloodworkService _bloodworkService;
        private readonly IChatService _chatServis;


        public JournalService(DatabaseContext context, IGoalService goalService, IUserService userService, IBloodworkService bloodworkService, IChatService chatServis)
        {
            _context = context;
            _goalService = goalService;
            _userService = userService;
            _bloodworkService = bloodworkService;
            _chatServis = chatServis;
        }

        
        public IEnumerable<JournalViewModel> GetJournals(string userSecretId)
        {
            var journals = _context.Journals
                                   .Where(j => j.UserID == userSecretId)
                                   .Include(j => j.BloodTests)
                                   .Include(j => j.Workouts)
                                   .Include(j => j.Goals)
                                   .Include(j => j.Chats)
                                   .ToList();

            
            var journalViewModels = journals.Select(journal => new JournalViewModel
            {
                JournalId = journal.Id,
                UserID = journal.UserID,
                JournalDate = journal.JournalDate,
                Title = journal.Title,
                Content = journal.Content,
                Resolved = journal.Resolved,
                SelectedBloodTestIds = journal.BloodTests.Select(bt => bt.Id).ToList(),
                SelectedWorkoutIds = journal.Workouts.Select(w => w.Id).ToList(),
                SelectedGoalIds = journal.Goals.Select(g => g.Id).ToList(),
                SelectedChatIds = journal.Chats.Select(c => c.Id).ToList(),
                
            });

            return journalViewModels;
        }

        public JournalDetailsViewModel GetJournalDetails(int journalId, string sid) {

            var user = _userService.FindUser(sid);

            if (user == null) {
                return null;
            }

            var journal = _context.Journals
                .Include(j => j.Workouts)
                .Include(j => j.BloodTests)
                .ThenInclude(bt => bt.Test)
                .Include(j => j.Goals)
                .Include(j => j.Chats)
                .FirstOrDefault(j => j.Id == journalId && j.UserID == user.Sid);


            if (journal == null)
            {
                return null;
            }

            var journalDetailsViewModels = new JournalDetailsViewModel 
            {   
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Birthday = user.Birthday.ToString("MM,dd,yyyy"),
                Age = user.Age,
                StartWeight = user.StartingWeight,
                CurrWeight = user.CurrentWeight,
                JournalID = journal.Id,
                Content = journal.Content,
                Title = journal.Title,
                JournalDate = journal.JournalDate.ToString("MM/dd/yyyy"),
                BloodTests = journal.BloodTests.Select(bt => new BloodTestViewModel
                {
                    Id = bt.Id,
                    SubmissionDate = bt.SubmissionDate,
                    BloodworkDate = bt.BloodworkDate,
                    Test = bt.Test.Select(t => new TestViewModel
                    {
                        Id = t.Id,
                        TestName = t.TestName,
                        Result = t.Result,
                        Grade = t.Grade
                    }).ToList()
            }).ToList(),
                Workouts = journal.Workouts.Select(w => new WorkoutViewModel 
                {
                    WorkoutId = w.Id,
                    userSecretId = w.UserID,
                    Type = w.Type,
                    SubType = w.SubType,
                    Set = w.Set,
                    Rep = w.Rep,
                    Day = w.Day,
                    resolved = w.resolved,
                    Duration = w.Duration
                }).ToList(),
                    Goals = journal.Goals.Select(g => new GoalViewModel 
                    {
                    GoalId = g.Id,
                    userSecretId = g.UserID,
                    resolved = g.resolved,
                    startingGoalDate = g.startingGoalDate,
                    endGoalDate = g.endGoalDate,
                    targetWeight = g.targetWeight,
                    Description = g.Description
                    }).ToList(),
                    Chats = journal.Chats.Select(c => new ChatViewModel 
                    {
                        Id = c.Id,
                        UserSID = c.UserSID,
                        ChatDate = c.ChatDate,
                        ChatTopic = c.ChatTopic
                    }).ToList()

        };

            return journalDetailsViewModels;

            
        }

        
        public void CreateJournal(JournalViewModel viewModel)
        {
            var newJournal = new Journal
            {
                UserID = viewModel.UserID,
                JournalDate = viewModel.JournalDate,
                Title = viewModel.Title,
                Content = viewModel.Content,
                Resolved = viewModel.Resolved,
                BloodTests = viewModel.SelectedBloodTestIds != null 
                            ? _context.BloodTests.Where(bt => viewModel.SelectedBloodTestIds.Contains(bt.Id)).ToList() 
                            : new List<BloodTest>(),
                Workouts = viewModel.SelectedWorkoutIds != null 
                            ? _context.Workouts.Where(w => viewModel.SelectedWorkoutIds.Contains(w.Id)).ToList() 
                            : new List<Workout>(),
                Goals = viewModel.SelectedGoalIds != null 
                            ? _context.Goals.Where(g => viewModel.SelectedGoalIds.Contains(g.Id)).ToList() 
                            : new List<Goal>(),
                Chats = viewModel.SelectedChatIds != null 
                            ? _context.Chats.Where(c => viewModel.SelectedChatIds.Contains(c.Id)).ToList() 
                            : new List<Chat>()
            };

            _context.Journals.Add(newJournal);
            _context.SaveChanges();
        }


        
        

        public void UpdateJournal(Journal journal)
        {
        //  var updatedJournal = _context.Goals.FirstOrDefault(g => g.Id == journal.Id);
        }


      public void DeleteJournal(int journalId)
      {
          var journal = _context.Journals.Find(journalId);
          if (journal != null)
          {
        journal.Resolved = true;
        _context.SaveChanges();
          }
      }

       

        
    }
}