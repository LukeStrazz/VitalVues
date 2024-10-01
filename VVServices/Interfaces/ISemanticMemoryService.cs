using Data.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces;

public interface ISemanticMemoryService
{
    Task<bool> UpdateMemory(string userSid, string memoryType, string content);
    Task<List<SemanticMemoryDto>> GetMemoryByUserSid(string userSid);
}
