using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Services.Interfaces;
using System.Net.Http.Headers;
using VitalVues.Models;
using System.Text;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/Chat")]
public class ChatController : Controller
{
    private readonly ILogger<ChatController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IChatService _chatService;
    private readonly IBloodworkService _bloodworkService;
    private readonly IGoalService _goalService;
    private readonly IFastingService _fastingService;

    public ChatController(ILogger<ChatController> logger, IHttpClientFactory clientFactory, IConfiguration configuration, IChatService chatService, IBloodworkService bloodworkService, IGoalService goalService, IFastingService fastingService)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _chatService = chatService;
        _bloodworkService = bloodworkService;
        _goalService = goalService;
        _fastingService = fastingService;
    }

    [HttpGet("Chat")]
    public IActionResult Chat()
    {
        return View();
    }


    [HttpPost]
    [Route("ChatConnector")]
    public IActionResult ChatConnector([FromBody] string content)
    {
        var apiKey = _configuration["API_KEY"];

        if (apiKey == null)
        {
            return Json(new { error = false, message = "Could not connect to services right now." });
        }

        var response = _chatService.GetChatResponse(apiKey, "");

        if (response == null)
        {
            string errorResponse = "Unable to get response.";
            return Json(new { success = false, message = errorResponse });
        }

        return Json(new { success = false, message = response });
    }

    public async Task<string> GetChatResponse(string request)
    {
        var client = _clientFactory.CreateClient();
        var apiKey = _configuration["API_KEY"];
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
