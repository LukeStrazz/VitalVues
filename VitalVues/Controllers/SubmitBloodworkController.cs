using Services.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;
using UglyToad.PdfPig;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace VitalVues.Controllers;


[ApiController]
[Route("api/PDFController")]
public class SubmitBloodworkController : Controller
{
    private readonly ILogger<SubmitBloodworkController> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly IChatService _chatService;

    public SubmitBloodworkController(ILogger<SubmitBloodworkController> logger, IHttpClientFactory clientFactory, IConfiguration configuration, IChatService chatService)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _chatService = chatService;
    }

    [HttpGet("PDF")]
    public IActionResult SubmitBloodwork()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadPdf(IFormFile pdfFile)
    {
        if (pdfFile == null || pdfFile.Length == 0)
            return Json(new { success = false, message = "No file selected or file is empty" });

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);

        }

        var filePath = Path.Combine(uploadPath, pdfFile.FileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await pdfFile.CopyToAsync(fileStream);
        }

        var pdfContentList = new List<string>();

        using (PdfDocument document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                pdfContentList.Add(page.Text);
            }
        }

        string combinedPdfContent = string.Join("\n", pdfContentList);

        var apiKey = _configuration["API_KEY"];

        if(apiKey != null)
        {
            return Json(new { success = false, message = "Could not connect to services right now." });
        }

        var response = await _chatService.GetChatResponse(apiKey, combinedPdfContent);

        return Json(new { success = true, message = "File uploaded successfully", content = response });

    }

 

}