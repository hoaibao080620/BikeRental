using Aggregator.Dto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Route("[controller]/[action]")]
// [Authorize]
public class DashboardController : ControllerBase
{
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;
    private readonly AccountServiceGrpc.AccountServiceGrpcClient _accountServiceGrpc;
    private readonly BikeBookingServiceGrpc.BikeBookingServiceGrpcClient _bikeBookingServiceGrpc;

    public DashboardController(GrpcClientFactory grpcClientFactory)
    {
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
}
