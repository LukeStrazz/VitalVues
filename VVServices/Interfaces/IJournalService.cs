using VVData.Data.Models;
using Services.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Data.Models;

namespace Services.Interfaces
{
    public interface IJournalService
    {
        public IEnumerable<JournalViewModel> GetJournals(string userSecretId);
        public void CreateJournal(JournalViewModel journalViewModel);
        public void UpdateJournal(Journal journal);
        public void DeleteJournal(int journalId);
        public JournalDetailsViewModel GetJournalDetails(int journalId, string sid);
    }
}