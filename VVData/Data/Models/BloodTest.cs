using Data.Data.Models;

namespace VVData.Data.Models;

public class BloodTest : TrackableEntry
{
    public DateTime SubmissionDate { get; set; }
    public DateTime BloodworkDate { get; set; }
    public List<Test> Test {  get; set; }
    public List<Journal> Journals { get; set; } = new List<Journal>();
}