using Data.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVData.Data;

namespace Services.Services;
public class SemanticMemoryService : ISemanticMemoryService
{
    private readonly DatabaseContext _dbContext;

    public SemanticMemoryService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> UpdateMemory(string userSid, string memoryType, string content)
    {
        Kernel kernel = new Kernel();
        
        
        var existingMemory = await _dbContext.SemanticMemory.Where(m => m.UserSid == userSid && m.MemoryType == memoryType).FirstOrDefaultAsync();

        if (existingMemory != null)
        {
            existingMemory.Content = content;
        }
        else
        {
            var newMemory = new SemanticMemoryDto
            {
                UserSid = userSid,
                MemoryType = memoryType,
                Content = content
            };
            await _dbContext.SemanticMemory.AddAsync(newMemory);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<List<SemanticMemoryDto>> GetMemoryByUserSid(string userSid)
    {
        return await _dbContext.SemanticMemory
            .Where(m => m.UserSid == userSid)
            .Select(m => new SemanticMemoryDto
            {
                MemoryType = m.MemoryType,
                Content = m.Content
            })
            .ToListAsync();
    }
}
