using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;
using VVData.Data.Models;

namespace Services.Interfaces
{
    public interface IFastingService
    {
        public void SaveFast(FastingViewModel fastingViewModel);
        public Fast GetLatestFastingSession(string userID); 
        public void ResetFastingSession(string userID);

    }
}
