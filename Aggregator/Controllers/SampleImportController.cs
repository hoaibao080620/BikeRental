using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleImportController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SampleImportController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IActionResult> DownloadImportSample(string? importType = "user")
    {
        var client = _httpClientFactory.CreateClient("s3");
        var url = importType == "user" ? "/import_user.csv" : "/import_bike.csv";
        var response = await client.GetStreamAsync(url);
        await using var memoryStream = new MemoryStream();
        await response.CopyToAsync(memoryStream);
        Response.Headers.Add("Content-Disposition", $"attachment;filename={url.Replace("/", string.Empty)}");
        return new FileStreamResult(memoryStream, 
            System.Net.Mime.MediaTypeNames.Application.Octet);
    }
}
