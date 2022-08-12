using System.Globalization;
using System.Security.Claims;
using Aggregator.Dto;
using Aggregator.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
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
    private readonly NotificationServiceGrpc.NotificationServiceGrpcClient _notificationService;

    public DashboardController(GrpcClientFactory grpcClientFactory, IViewRender viewRender, IWebHostEnvironment env)
    {
        _viewRender = viewRender;
        _env = env;
        _bikeServiceGrpc = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
        _accountServiceGrpc = grpcClientFactory.CreateClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService");
        _bikeBookingServiceGrpc = grpcClientFactory.CreateClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>(
            "BikeBookingService");
        _notificationService = grpcClientFactory.CreateClient<NotificationServiceGrpc.NotificationServiceGrpcClient>(
            "NotificationService");
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
    public async Task<IActionResult> GetCallRating()
    {
        var list = new List<CallChart>();
        var callRating = await _notificationService.GetCallStatisticAsync(new Empty());
        list.Add(new CallChart
        {
            CallType = "Incoming",
            Percentage = callRating.IncomingCallRate
        });
        
        list.Add(new CallChart
        {
            CallType = "Outgoing",
            Percentage = callRating.OutgoingCallRate
        });
        return Ok(list);
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
        var url = Environment.GetEnvironmentVariable("NGROK_URL");
        if (_env.IsProduction())
        {
            return Redirect($"{url}/dashboard/downloadReport?filterType={filterType}");
        }

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
        
        IronPdf.License.LicenseKey = "IRONPDF.LÊBÃO.5806-FD613F2C1E-FK7DYDE4MMNRQ-XGVM3KMPOVIG-7CRNBPBNWYUX-MIDUKVKQKIWE-WGJZGKYYTBA7-T4BFKB-TWFDEUF6DT2HEA-DEPLOYMENT.TRIAL-6TOQKN.TRIAL.EXPIRES.06.SEP.2022";
            
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
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetManagerDashboard()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var bikeStatistic = await _bikeServiceGrpc.GetManagerDashboardStatisticAsync(
            new GetManagerDashboardStatisticRequest
            {
                ManagerEmail = email
            });

        var rentingCount = await _bikeBookingServiceGrpc.GetBikeRentingCountAsync(new GetBikeRentingCountRequest
        {
            BikeIds = {bikeStatistic.BikeIds}
        });

        return Ok(new ManagerDashboard
        {
            BikeCount = bikeStatistic.TotalBike,
            BikeReportCount = bikeStatistic.TotalBikeReport,
            StationCount = bikeStatistic.TotalBikeStation,
            TotalRenting = rentingCount.TotalRenting
        });
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
