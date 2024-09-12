using Microsoft.AspNetCore.Mvc;
using System;
namespace Services.Interfaces;

public interface IChatService
{
	public Task<string> GetChatResponse(string apiKey, string request);
}

