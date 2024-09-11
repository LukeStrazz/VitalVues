using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Services.Services;

public class ChatService : IChatService
{
    private readonly ILogger<ChatService> _logger;
    private readonly IHttpClientFactory _clientFactory;

    public ChatService(ILogger<ChatService> logger, IHttpClientFactory clientFactory)
	{
        _logger = logger;
        _clientFactory = clientFactory;
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
            model = "gpt-4",
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

            // Ensure the response structure is correct
            var reply = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();
            if (reply != null)
            {
                return reply;
            }
            return "Response is not valid. Try again later.";
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return "Error from API: {errorContent}" + errorContent;
        }
    }
}

