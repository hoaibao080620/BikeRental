using System.Globalization;
using BikeBookingService.DAL;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Shared.Service;
using TimeZone = Shared.Consts.TimeZone;

namespace BikeBookingService.GrpcServices;

public class BikeBookingGrpcService : BikeBookingServiceGrpc.BikeBookingServiceGrpcBase
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeBookingGrpcService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public override async Task<GetStatisticsResponse> GetBikeRentingStatistics(GetStatisticsRequest request, ServerCallContext context)
    {
        double totalRenting = 0;
        double previousTotalRenting = 0;
        var now = DateTime.UtcNow;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn >= firstDateOfWeek.Date &&
                        x.CreatedOn.Date <= now
                    )).Count();

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek).AddDays(1).AddTicks(-1);
                previousTotalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn <= filterDateOfPreviousWeek
                    )).Count();
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn >= firstDateOfMonth.Date &&
                        x.CreatedOn <= now
                    )).Count();

                var previousMonth = now.AddMonths(-1).AddDays(1).AddTicks(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn <= previousMonth
                    )).Count();
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn >= firstDateOfYear.Date &&
                        x.CreatedOn <= now
                    )).Count();

                var previousYear = now.AddYears(-1).AddDays(1).AddTicks(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn <= previousYear
                    )).Count();
                break;
        }
        
        double rateCompare;
        if (previousTotalRenting == 0)
        {
            rateCompare = totalRenting == 0 ? 0 : 100;
        }
        else
        {
            rateCompare = totalRenting / previousTotalRenting * 100 - 100;
        }

        return new GetStatisticsResponse
        {
            RateCompare = rateCompare,
            Total = totalRenting
        };
    }

    public override async Task<GetRentingChartDataResponse> GetBikeRentingChartData(Empty request, ServerCallContext context)
    {
        var today = DateTime.UtcNow;
        var dayOfWeek = today.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) today.DayOfWeek;
        var chartData = new List<int>();

        for (var i = 1; i <= dayOfWeek; i++)
        {
            var dateByDateOfWeek = today.Subtract(TimeSpan.FromDays(dayOfWeek - i)).Date;
            var totalByDate = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.CheckoutOn != null && x.UpdatedOn.HasValue
                                                && x.UpdatedOn.Value.Date == dateByDateOfWeek)).Count();
            
            chartData.Add(totalByDate);
        }

        return new GetRentingChartDataResponse
        {
            ChartData = {chartData}
        };
    }

    public override async Task<GetTopThreeBikeHasBeenRentResponse> GetTopThreeBikeHasBeenRent(Empty request, ServerCallContext context)
    {
        var topThreeBikeRent = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.CheckoutOn != null && x.Bike.IsActive))
            .GroupBy(x => new
            {
                x.BikeId,
                BikeLicensePlate = x.Bike.BikeCode
            })
            .Select(x => new BikeRent
            {
                BikeId = x.Key.BikeId,
                BikeLicensePlate = x.Key.BikeLicensePlate,
                TotalRentingTimes = x.Count(),
                TotalRentingPoint = x.Sum(xx => xx.TotalPoint)
            });

        return new GetTopThreeBikeHasBeenRentResponse
        {
            TopThreeBikeRent = { topThreeBikeRent }
        };
    }

    public override async Task<GetTopThreeAccountRentingResponse> GetTopThreeAccountRenting(Empty request, ServerCallContext context)
    {
        var topThreeAccountRent = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.CheckoutOn != null && x.Bike.IsActive))
            .GroupBy(x => x.Account.PhoneNumber)
            .Select(x => new AccountRent
            {
                PhoneNumber = x.Key,
                TotalRentingTimes = x.Count(),
                TotalRentingPoint = x.Sum(xx => xx.TotalPoint)
            });

        return new GetTopThreeAccountRentingResponse
        {
            TopThreeAccountRent = {topThreeAccountRent}
        };
    }

    public override async Task<GetTotalTimesRentingByBikeStationResponse> GetTotalTimesRentingByBikeStation(Empty request, ServerCallContext context)
    {
        var bikeRentals = await _unitOfWork.BikeRentalTrackingRepository
            .Find(x => x.CheckoutOn != null);

        var totalBikeRental = bikeRentals.Count();

        if (totalBikeRental > 0)
        {
            var result = bikeRentals
                .GroupBy(x => x.CheckinBikeStation)
                .Select(x => new TotalTimesRentingByBikeStation
                {
                    BikeStation = x.Key,
                    Percentage = x.Count() * 1.0 / totalBikeRental * 100
                });

            return new GetTotalTimesRentingByBikeStationResponse
            {
                Result = { result }
            };
        }
        
        return new GetTotalTimesRentingByBikeStationResponse
        {
            Result = { Array.Empty<TotalTimesRentingByBikeStation>() }
        };
    }

    public override async Task<GetRentingInfoResponse> GetRentingInfo(GetRentingInfoRequest request, ServerCallContext context)
    {
        var totalDistance = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.Account.Email == request.Email))
            .Select(x => x.BikeLocationTrackingHistories.Sum(xx => xx.DistanceFromPreviousLocation))
            .Sum();

        var totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
            .Find(x => x.Account.Email == request.Email)).Count();

        return new GetRentingInfoResponse
        {
            TotalDistance = totalDistance,
            TotalRenting = totalRenting
        };
    }
}
