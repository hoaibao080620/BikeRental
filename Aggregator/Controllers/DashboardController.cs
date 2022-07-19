using Aggregator.Dto;
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
        // var getRevenueStatistic = await _accountServiceGrpc.GetPaymentStatisticsAsync(new AccountGetStatisticsRequest
        // {
        //     FilterType = filterType
        // });
        
        var getAccountStatistic = await _accountServiceGrpc.GetAccountStatisticsAsync(new AccountGetStatisticsRequest
        {
            FilterType = filterType
        });

        var getBikeBookingStatistics = await _bikeBookingServiceGrpc.GetBikeRentingStatisticsAsync(
            new BikeBookingGetStatisticsRequest
            {
                FilterType = filterType
            });

        var getBikeReportStatistics = await _bikeServiceGrpc.GetBikeReportStatisticsAsync(new BikeGetStatisticsRequest
        {
            FilterType = filterType
        });

        // await Task.WhenAll(
        //     getRevenueStatistic.ResponseAsync, 
        //     getAccountStatistic.ResponseAsync,
        //     getBikeBookingStatistics.ResponseAsync,
        //     getBikeReportStatistics.ResponseAsync
        // );
        //
        // return Ok(new NumberStatisticDto
        // {
        //     TotalRevenue = getRevenueStatistic.ResponseAsync.Result.Total,
        //     RevenueRateCompare = getRevenueStatistic.ResponseAsync.Result.RateCompare,
        //     TotalAccount = getAccountStatistic.ResponseAsync.Result.Total,
        //     AccountRateCompare = getAccountStatistic.ResponseAsync.Result.RateCompare,
        //     TotalBikeRental = getBikeBookingStatistics.ResponseAsync.Result.Total,
        //     BikeRentalRateCompare = getBikeBookingStatistics.ResponseAsync.Result.RateCompare,
        //     TotalBikeReport = getBikeReportStatistics.ResponseAsync.Result.Total,
        //     BikeReportRateCompare = getBikeReportStatistics.ResponseAsync.Result.RateCompare
        // });

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetRentingChart()
    {
        var rentingChartData = await _bikeBookingServiceGrpc.GetBikeRentingChartDataAsync(new Empty());
        return Ok(rentingChartData.ChartData.ToList());
    }
}
