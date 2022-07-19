using System.Globalization;
using BikeBookingService.DAL;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

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
        var now = DateTime.Now;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfWeek.Date &&
                        x.CreatedOn.Date <= now.Date
                    )).Count();

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek);
                previousTotalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn.Date <= filterDateOfPreviousWeek.Date
                    )).Count();
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfMonth.Date &&
                        x.CreatedOn.Date <= now.Date
                    )).Count();

                var previousMonth = now.AddMonths(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn.Date <= previousMonth.Date
                    )).Count();
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfYear.Date &&
                        x.CreatedOn.Date <= now.Date
                    )).Count();

                var previousYear = now.AddYears(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalRenting = (await _unitOfWork.BikeRentalTrackingRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn.Date <= previousYear.Date
                    )).Count();
                break;
        }

        return new GetStatisticsResponse
        {
            RateCompare = previousTotalRenting == 0 ? 100 : totalRenting / previousTotalRenting * 100 - 100,
            Total = totalRenting
        };
    }

    public override async Task<GetRentingChartDataResponse> GetBikeRentingChartData(Empty request, ServerCallContext context)
    {
        var today = DateTime.Now;
        var dayOfWeek = today.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) today.DayOfWeek;
        var chartData = new List<int>();

        for (var i = 1; i <= dayOfWeek; i++)
        {
            var dateByDateOfWeek = today.Subtract(TimeSpan.FromDays(dayOfWeek - i)).Date;
            var totalByDate = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.CheckoutOn == null && x.CreatedOn.Date == dateByDateOfWeek)).Count();
            
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
                .Find(x => x.CheckoutOn == null && x.Bike.IsActive))
            .GroupBy(x => new
            {
                x.BikeId,
                BikeLicensePlate = x.Bike.LicensePlate
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
                .Find(x => x.CheckoutOn == null && x.Bike.IsActive))
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
            .Find(x => x.CheckoutOn == null && x.Bike.IsActive && x.Bike.BikeStationId.HasValue);

        var totalBikeRental = bikeRentals.Count();
            
        var result = bikeRentals
            .GroupBy(x => x.Bike.BikeStationId)
            .Select(x => new TotalTimesRentingByBikeStation
            {
                BikeStationId = x.Key!.Value,
                Percentage = x.Count() * 1.0 / totalBikeRental 
            });

        return new GetTotalTimesRentingByBikeStationResponse
        {
            Result = { result }
        };
    }
}
