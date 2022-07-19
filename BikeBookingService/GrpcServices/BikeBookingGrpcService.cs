using System.Globalization;
using BikeBookingService.DAL;
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
}
