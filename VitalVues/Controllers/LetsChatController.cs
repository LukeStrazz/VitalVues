using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Services.Interfaces;
using System.Net.Http.Headers;
using VitalVues.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Azure;
using Services.ViewModels;

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

    [NoCacheHeaders]
    [HttpGet("Chat")]
    public IActionResult Chat()
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        if (string.IsNullOrEmpty(userUniqueIdentifier))
        {
            return RedirectToAction("Error", "Home");
        }

        var chats = _chatService.GetChats(userUniqueIdentifier).ToList();
        var bloodworks = _bloodworkService.GetBloodworks(userUniqueIdentifier).ToList();
        var userInfo = new UserInfoViewModel
        {
            Sid = userUniqueIdentifier,
            Chats = chats,
            Bloodworks = bloodworks
        };

        return View(userInfo);
    }

    [HttpPost]
    [Route("GetChatResponse")]
    public async Task<IActionResult> GetChatResponse([FromBody] ChatRequest requests)
    {
        var client = _clientFactory.CreateClient();
        var messages = string.Join(",", requests.Messages.Select(m => m.Content));
    

        var apiKey = _configuration["API_KEY"];
        var response = await _chatService.GetChatResponse(apiKey, messages);
        if (response == null)
        {
            string errorResponse = "Unable to get response.";
            return Json(new { success = false, message = errorResponse });
        }

        try
        {
            var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
            return Json(new { success = true, message = jsonResponse });
        }
        catch (Exception ex)
        {
            return Json(new { success = true, message = response });
        }
    }



    [HttpPost]
    [Route("SaveChats")]
    public async Task<IActionResult> SaveChats(ChatViewModel messages)
    {
        var userUniqueIdentifier = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

        _chatService.SaveChat(userUniqueIdentifier, messages);
        return Json(new { success = true, message = "Chat saved!" });
    }
}
