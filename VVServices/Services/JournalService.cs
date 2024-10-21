using System.Collections.Generic;
using System.Linq;
using VVData.Data.Models;
using Services.ViewModels;
using Services.Interfaces;
using VVData.Data;
using Microsoft.EntityFrameworkCore;
using Data.Data.Models;


namespace Services
{
    public class JournalService : IJournalService
    {
        private readonly DatabaseContext _context;

        public JournalService(DatabaseContext context)
        {
            _context = context;
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