using Microsoft.AspNetCore.Mvc;
using Services.ViewModels;
using System;
namespace Services.Interfaces;

public interface IChatService
{
    IEnumerable<ChatViewModel> GetChats(string userSecretId);
    public Task<string> GetChatResponse(string apiKey, string request);
    void SaveChat(string? userUniqueIdentifier, ChatViewModel messages);
}

