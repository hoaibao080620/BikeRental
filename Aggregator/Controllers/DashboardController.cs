using System.Globalization;
using Aggregator.Dto;
using Aggregator.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Route("[controller]/[action]")]
// [Authorize]
public class DashboardController : ControllerBase
{
    private readonly IViewRender _viewRender;
    private readonly IWebHostEnvironment _env;
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;
    private readonly AccountServiceGrpc.AccountServiceGrpcClient _accountServiceGrpc;
    private readonly BikeBookingServiceGrpc.BikeBookingServiceGrpcClient _bikeBookingServiceGrpc;

    public DashboardController(GrpcClientFactory grpcClientFactory, IViewRender viewRender, IWebHostEnvironment env)
    {
        _viewRender = viewRender;
        _env = env;
        _bikeServiceGrpc = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
        _accountServiceGrpc = grpcClientFactory.CreateClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService");
        _bikeBookingServiceGrpc = grpcClientFactory.CreateClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>(
            "BikeBookingService");
    }

    [HttpGet]
    public async Task<IActionResult> GetNumberStatistics([FromQuery] string? filterType = "week")
    {
        var getRevenueStatistic = _accountServiceGrpc.GetPaymentStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });
        
        var getAccountStatistic = _accountServiceGrpc.GetAccountStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });

        var getBikeBookingStatistics = _bikeBookingServiceGrpc.GetBikeRentingStatisticsAsync(
            new BikeBookingGetStatisticsRequest
            {
                FilterType = filterType
            });

        var getBikeReportStatistics = _bikeServiceGrpc.GetBikeReportStatisticsAsync(new BikeGetStatisticsRequest
        {
            FilterType = filterType
        });

        await Task.WhenAll(
            getRevenueStatistic.ResponseAsync, 
            getAccountStatistic.ResponseAsync,
            getBikeBookingStatistics.ResponseAsync,
            getBikeReportStatistics.ResponseAsync
        );
        
        return Ok(new NumberStatisticDto
        {
            TotalRevenue = getRevenueStatistic.ResponseAsync.Result.Total,
            RevenueRateCompare = getRevenueStatistic.ResponseAsync.Result.RateCompare,
            TotalAccount = getAccountStatistic.ResponseAsync.Result.Total,
            AccountRateCompare = getAccountStatistic.ResponseAsync.Result.RateCompare,
            TotalBikeRental = getBikeBookingStatistics.ResponseAsync.Result.Total,
            BikeRentalRateCompare = getBikeBookingStatistics.ResponseAsync.Result.RateCompare,
            TotalBikeReport = getBikeReportStatistics.ResponseAsync.Result.Total,
            BikeReportRateCompare = getBikeReportStatistics.ResponseAsync.Result.RateCompare
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetRentingChart(string filterType = "week")
    {
        var (startDate, endDate) = GetFilterDate(filterType);
        var rentingChartData = await _bikeBookingServiceGrpc.GetBikeRentingChartDataAsync(new BikeBookingGetStatisticsRequest
        {
            FilterType = filterType,
            StartDate = startDate.ToTimestamp(),
            EndDate = endDate.ToTimestamp()
        });
        
        return Ok(rentingChartData.ChartData.ToList());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRecentTransactions()
    {
        var data = await _accountServiceGrpc.GetRecentTransactionsAsync(new GetRecentTransactionsRequest
        {
            NumberOfItem = 4
        });
        return Ok(data.Transactions.ToList());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTotalRentByBikeStation()
    {
        var data = await _bikeBookingServiceGrpc.GetTotalTimesRentingByBikeStationAsync(new Empty());
        foreach (var val in data.Result.ToList())
        {
            val.Percentage = Math.Round(val.Percentage, 2, MidpointRounding.ToZero);
        }

        double currentPercent = 0.0;
        for (var i = 0; i < data.Result.Count; i++)
        {
            if (i == data.Result.Count - 1)
            {
                data.Result[i].Percentage = 100 - currentPercent;
            }
            else
            {
                data.Result[i].Percentage = Math.Round(data.Result[i].Percentage, 2, MidpointRounding.ToZero);
                currentPercent += data.Result[i].Percentage;
            }
        }
        
        return Ok(data.Result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTopThreeAccountReting()
    {
        var data = await _bikeBookingServiceGrpc.GetTopThreeAccountRentingAsync(new Empty());
        return Ok(data.TopThreeAccountRent.OrderByDescending(x => x.TotalRentingPoint)
            .ThenByDescending(x => x.TotalRentingTimes));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTopThreeBikeRent()
    {
        var data = await _bikeBookingServiceGrpc.GetTopThreeBikeHasBeenRentAsync(new Empty());
        return Ok(data.TopThreeBikeRent.OrderByDescending(x => x.TotalRentingPoint)
            .ThenByDescending(x => x.TotalRentingTimes));
    }
    
    [HttpGet]
    public async Task<IActionResult> DownloadReport(string? filterType = "week")
    {
        var (startDate, endDate) = GetFilterDate(filterType!);
        var reportDisplayDict = new Dictionary<string, string>
        {
            {"week", "tuần"},
            {"month", "tháng"},
            {"year", "năm"}
        };
        
        var chartColumnDict = new Dictionary<string, List<string>>
        {
            {"week", new List<string>
            {
                "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"
            }},
            {"month", new List<string>
            {
                "Tuần 1", "Tuần 2", "Tuần 3", "Tuần 4"
            }},
            {"year", new List<string>
            {
                "Tháng 1", 
                "Tháng 2",
                "Tháng 3",
                "Tháng 4",
                "Tháng 5",
                "Tháng 6",
                "Tháng 7",
                "Tháng 8",
                "Tháng 9",
                "Tháng 10",
                "Tháng 11",
                "Tháng 12",
            }}
        };
    
        var chartData = _accountServiceGrpc.GetPaymentChartAsync(new GetPaymentChartRequest
        {
            FilterType = filterType,
            StartDate = startDate.ToTimestamp(),
            EndDate = endDate.ToTimestamp()
        });
    
        var paymentStatistic = _accountServiceGrpc.GetPaymentStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });
        
        var accountStatistics = _accountServiceGrpc.GetAccountStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });
        
        var bikeBookingStatistics = _bikeBookingServiceGrpc.GetBikeRentingStatisticsAsync(
            new BikeBookingGetStatisticsRequest
            {
                FilterType = filterType
            });
    
        var bikeReportStatistics = _bikeServiceGrpc.GetBikeReportStatisticsAsync(new BikeGetStatisticsRequest
        {
            FilterType = filterType
        });
    
        await Task.WhenAll(
            paymentStatistic.ResponseAsync,
            accountStatistics.ResponseAsync,
            bikeBookingStatistics.ResponseAsync,
            bikeReportStatistics.ResponseAsync,
            chartData.ResponseAsync);
    
        var htmlContent = await _viewRender.RenderPartialViewToString("report", new ReportExportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ReportType = reportDisplayDict[filterType!],
            TotalTransaction = (int) paymentStatistic.ResponseAsync.Result.TotalCount,
            Revenue = paymentStatistic.ResponseAsync.Result.Total,
            TotalAccount = (int) accountStatistics.ResponseAsync.Result.Total,
            TotalBooking = (int) bikeBookingStatistics.ResponseAsync.Result.Total,
            TotalBikeReport = (int) bikeReportStatistics.ResponseAsync.Result.Total,
            ChartData = chartData.ResponseAsync.Result.ChartData.ToList(),
            ChartColumns = chartColumnDict[filterType!]
        });
        IronPdf.License.LicenseKey = Environment.GetEnvironmentVariable("IRON_PDF_KEY");
            
        var renderer = new IronPdf.ChromePdfRenderer
        {
            RenderingOptions =
            {
                EnableJavaScript = true,
                RenderDelay = 1000,
                CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print
            }
        };
        
        var pdfDoc = renderer.RenderHtmlAsPdf(htmlContent);
        pdfDoc.RemovePage(1);
        return File(pdfDoc.BinaryData, 
            System.Net.Mime.MediaTypeNames.Application.Pdf, 
            $"report_{DateTime.Now.ToShortDateString()}.pdf");
    }

    private (DateTime StartDate, DateTime EndDate) GetFilterDate(string filterType)
    {
        var now = DateTime.Now;
        var weekNumber = ISOWeek.GetWeekOfYear(now);
        var startDate = new DateTime();
        var endDate = new DateTime();
        
        switch (filterType)
        {
            case "week":
                startDate = ISOWeek.ToDateTime(now.Year, weekNumber, DayOfWeek.Monday);
                endDate = ISOWeek.ToDateTime(now.Year, weekNumber, DayOfWeek.Sunday);
                break;
            case "month":
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
                break;
            case "year":
                startDate = new DateTime(now.Year, 1, 1);
                endDate = new DateTime(now.Year, 12, 31);
                break;
        }

        return (DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc), DateTime.SpecifyKind(endDate.Date, DateTimeKind.Utc));
    }
}
