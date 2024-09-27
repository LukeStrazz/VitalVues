using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ViewModels
{
    public class SubmitBloodworkRequest
    {
        public DateTime SubmissionDate { get; set; }
        public List<TestResultViewModel> Tests { get; set; }
    }
}
