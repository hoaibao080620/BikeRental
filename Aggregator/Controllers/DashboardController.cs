using System.Globalization;
using System.Text;
using Aggregator.Dto;
using Aggregator.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using iText.Html2pdf;

using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Route("[controller]/[action]")]
// [Authorize]
public class DashboardController : ControllerBase
{
    private readonly IViewRender _viewRender;
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;
    private readonly AccountServiceGrpc.AccountServiceGrpcClient _accountServiceGrpc;
    private readonly BikeBookingServiceGrpc.BikeBookingServiceGrpcClient _bikeBookingServiceGrpc;

    public DashboardController(GrpcClientFactory grpcClientFactory, IViewRender viewRender)
    {
        _viewRender = viewRender;
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
    public async Task<IActionResult> GetRentingChart()
    {
        var rentingChartData = await _bikeBookingServiceGrpc.GetBikeRentingChartDataAsync(new Empty());
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
        var now = DateTime.Now;
        var weekNumber = ISOWeek.GetWeekOfYear(now);
        var startDate = new DateTime();
        var endDate = new DateTime();
        var reportTypeDisplay = string.Empty;

        switch (filterType)
        {
            case "week":
                startDate = ISOWeek.ToDateTime(now.Year, weekNumber, DayOfWeek.Monday);
                endDate = ISOWeek.ToDateTime(now.Year, weekNumber, DayOfWeek.Sunday);
                reportTypeDisplay = "tuần";
                break;
            case "month":
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
                reportTypeDisplay = "tháng";
                break;
            case "year":
                startDate = new DateTime(now.Year, 1, 1);
                endDate = new DateTime(now.Year, 12, 31);
                reportTypeDisplay = "năm";
                break;
        }

        var paymentStatistic = await _accountServiceGrpc.GetPaymentStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });
        
        var accountStatistics = await _accountServiceGrpc.GetAccountStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });
        
        var bikeBookingStatistics = await _bikeBookingServiceGrpc.GetBikeRentingStatisticsAsync(
            new BikeBookingGetStatisticsRequest
            {
                FilterType = filterType
            });

        var bikeReportStatistics = await _bikeServiceGrpc.GetBikeReportStatisticsAsync(new BikeGetStatisticsRequest
        {
            FilterType = filterType
        });

        var htmlContent = await _viewRender.RenderPartialViewToString("report", new ReportExportDto
        {
            StartDate = startDate,
            EndDate = endDate,
            ReportType = reportTypeDisplay,
            TotalTransaction = paymentStatistic.TotalCount,
            Revenue = paymentStatistic.Total,
            TotalAccount = (int) accountStatistics.Total,
            TotalBooking = (int) bikeBookingStatistics.Total,
            TotalBikeReport = (int) bikeReportStatistics.Total
        });

        var memoryStream = new MemoryStream();
        HtmlConverter.ConvertToPdf(htmlContent, memoryStream);
        
        return File(memoryStream.ToArray(), "application/pdf", 
            $"report_{DateTime.Now.ToShortDateString()}.pdf");
    }
}
