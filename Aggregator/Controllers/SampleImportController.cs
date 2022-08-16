using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Route("[controller]")]
public class SampleImportController : ControllerBase
{
    public IActionResult DownloadImportSample(string? importType = "user")
    {
        return importType switch
        {
            "user" => Redirect("https://bike-rental-fe.s3.amazonaws.com/import_user.csv"),
            "bike" => Redirect("https://bike-rental-fe.s3.amazonaws.com/import_bike.csv"),
            _ => BadRequest("Không tìm thấy bản sample!")
        }; 
    }
}
