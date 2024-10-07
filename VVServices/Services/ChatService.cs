using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults;
using Services.ViewModels;
using VVData.Data;
using Data.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Services.Services;

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly DatabaseContext _context;

    public ChatService(ILogger<ChatService> logger, DatabaseContext context, IHttpClientFactory clientFactory)
	{
        _logger = logger;
        _context = context;
        _clientFactory = clientFactory;
    }

    public IEnumerable<ChatViewModel> GetChats(string userSecretId)
    {
        var user = _context.People.FirstOrDefault(g => g.Sid == userSecretId);

        var chats = _context.Chats.Where(g => g.UserSID == userSecretId);

        if (chats == null)
        {
            return Enumerable.Empty<ChatViewModel>();
        }

        var viewModelChats = new List<ChatViewModel>();
        foreach (var chat in chats)
        {
            var chatToAdd = new ChatViewModel
            {
                Id = chat.Id,
                UserSID = chat.UserSID,
                ChatDate = chat.ChatDate,
                ChatTopic = chat.ChatTopic,
                Messages = chat.Messages,
            };

            viewModelChats.Add(chatToAdd);
        }

        return viewModelChats;
    }

    public async Task<string> GetChatResponse(string apiKey, string request)
    {
        var client = _clientFactory.CreateClient();

        if (string.IsNullOrEmpty(apiKey))
        {
            return "Null API Key.";
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "user", content = request }
            }
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseContent);
            var reply = jsonResponse["choices"][0]["message"]["content"].ToString();
            return reply.ToString();
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return "Error from API: {errorContent}" + errorContent;
        }
    }

    public int SaveChat(int? chatId, string? userUniqueIdentifier, ChatViewModel chatViewModel)
    {
        var existingChat = _context.Chats.Include(c => c.Messages).FirstOrDefault(c => c.Id == chatId);

        if (existingChat == null)
        {
            var messageList = new List<Message>();
            foreach (var message in chatViewModel.Messages)
            {
                var existingMessage = _context.Messages.FirstOrDefault(m => m.content == message.content && m.role == message.role);

                if (existingMessage == null)
                {
                    var newMessage = new Message { role = message.role, content = message.content };
                    messageList.Add(newMessage);
                    _context.Add(newMessage); 
                }
            }
            var today = DateTime.Today;
            var newChat = new Chat
            {
                UserSID = userUniqueIdentifier,
                ChatDate = today,
                ChatTopic = chatViewModel.ChatTopic,
                Messages = messageList
            };

            _context.Add(newChat);
            _context.SaveChanges();
            return newChat.Id;
        }
        else
        {
            foreach (var message in chatViewModel.Messages)
            {
                var existingMessage = existingChat.Messages.FirstOrDefault(m => m.content == message.content && m.role == message.role);

                if (existingMessage == null)
                {
                    existingChat.Messages.Add(new Message { role = message.role, content = message.content });
                }
            }
            _context.SaveChanges();
            return existingChat.Id;
        }

    }

}

