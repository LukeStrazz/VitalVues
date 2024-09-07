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


    public SubmitBloodworkController(ILogger<SubmitBloodworkController> logger, IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
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

        var response = await GetChatResponse(combinedPdfContent);

        return Json(new { success = true, message = "File uploaded successfully", content = response });

    }

    public async Task<IActionResult> GetChatResponse([FromBody] string request)
    {
        var client = _clientFactory.CreateClient();
        var apiKey = Environment.GetEnvironmentVariable("API_KEY");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
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