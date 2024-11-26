using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Data.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VVData.Data;

public class BMIService : IBMIService
{
    private readonly DatabaseContext _context;

    public BMIService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<BMIProgress>> GetAllBMIProgressAsync(string userId)
    {
        return await _context.BMIProgress
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.RecordedAt)
            .ToListAsync();
    }

    public async Task<BMIProgress> AddBMIProgressAsync(string userId, double bmi)
    {
        var newRecord = new BMIProgress
        {
            UserId = userId,
            BMIValue = Math.Round(bmi, 2),
            RecordedAt = DateTime.Now
        };

        _context.BMIProgress.Add(newRecord);
        await _context.SaveChangesAsync();

        return newRecord;
    }

    public async Task<bool> DeleteBMIRecordAsync(int id, string userId)
    {
        var record = await _context.BMIProgress.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (record == null)
            return false;

        _context.BMIProgress.Remove(record);
        await _context.SaveChangesAsync();

        return true;
    }
}


