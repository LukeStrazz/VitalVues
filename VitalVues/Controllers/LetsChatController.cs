using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Services.Interfaces;
using System.Net.Http.Headers;
using VitalVues.Models;
using System.Text;

namespace VitalVues.Controllers;

[ApiController]
[Route("api/LetsChat")]
public class LetsChatController : Controller
{
    private readonly ILogger<LetsChatController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IChatService _chatService;
    private readonly IBloodworkService _bloodworkService;
    private readonly IGoalService _goalService;
    private readonly IFastingService _fastingService;

    public LetsChatController(ILogger<LetsChatController> logger, IHttpClientFactory clientFactory, IConfiguration configuration, IChatService chatService, IBloodworkService bloodworkService, IGoalService goalService, IFastingService fastingService)
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

        var response = _chatService.GetChatResponse(apiKey, content);

        if (response == null)
        {
            string errorResponse = "Unable to get response.";
            return Json(new { success = false, message = errorResponse });
        }

        return Json(new { success = false, message = response });
    }

    [HttpPost]
    [Route("GetChatResponse")]
    public async Task<IActionResult> GetChatResponse([FromBody] ChatRequest request)
    {
        var client = _clientFactory.CreateClient();
        var apiKey = _configuration["API_KEY"];
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            model = "gpt-4",
            messages = request.Messages.Select(m => new { role = m.Role, content = m.Content }).ToList()
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
        };

        var response = await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseContent);
            var reply = jsonResponse["choices"][0]["message"]["content"].ToString();
            return Json(new { message = reply });
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, "Error from API: " + errorContent);
        }
    }
}
