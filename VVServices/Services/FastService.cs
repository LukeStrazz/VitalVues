using System;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.ViewModels;
using VVData.Data;
using VVData.Data.Models;

namespace Services.Services
{
    public class FastService : IFastingService
    {
        private readonly ILogger<FastService> _logger;
        private readonly DatabaseContext _context;

        public FastService(ILogger<FastService> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void SaveFast(FastingViewModel fastingViewModel)
        {
            var newFast = new Fast
            {
                userID = fastingViewModel.userID,
                start = fastingViewModel.start.AddHours(-5),
                end = fastingViewModel.end.AddHours(-5)
            };

            _context.Fasts.Add(newFast);
            _context.SaveChanges();
        }
        public Fast GetLatestFastingSession(string userID)
        {
            var fastingSession = _context.Fasts
                .Where(f => f.userID == userID)
                .OrderByDescending(f => f.start)
                .FirstOrDefault();

            // Handle case where fastingSession is null if needed
            return fastingSession;
        }

        public void ResetFastingSession(string userID) 
        {
            var fastingSession = _context.Fasts
                .Where(f => f.userID == userID)
                .OrderByDescending(f => f.start)
                .FirstOrDefault();

            if (fastingSession != null)
            {
                _context.Fasts.Remove(fastingSession);
                _context.SaveChanges();
            }
        }
    }
}
