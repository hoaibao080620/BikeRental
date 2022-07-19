﻿using System.Globalization;
using BikeService.Sonic.DAL;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.GrpcServices;

public class BikeGrpcService : BikeServiceGrpc.BikeServiceGrpcBase
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeGrpcService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public override async Task<GetBikeIdsResponse> GetBikeIdsOfManager(GetBikeIdsRequest request, ServerCallContext context)
    {
        var manager = (await _unitOfWork.ManagerRepository
            .Find(x => x.Email == request.ManagerEmail)).FirstOrDefault();

        ArgumentNullException.ThrowIfNull(manager);
        
        var bikeIds = (await _unitOfWork.BikeRepository
            .Find(x =>
                x.BikeStationId.HasValue &&
                manager.IsSuperManager || 
                x.BikeStation!.BikeStationManagers.Any(b => b.Manager.Email == request.ManagerEmail)
            ))
            .Select(x => x.Id).ToList();

        return new GetBikeIdsResponse
        {
            BikeIds =
            {
                bikeIds
            }
        };
    }

    public override async Task<GetManagerEmailsResponse> GetManagerEmailsOfBikeId(GetManagerEmailsRequest request, ServerCallContext context)
    {
        var managerEmails = (await _unitOfWork.BikeStationManagerRepository
                .Find(x => x.BikeStation.Bikes.Any(b => b.Id == request.BikeId)))
            .AsNoTracking().Select(x => x.Manager.Email).ToList();

        return new GetManagerEmailsResponse
        {
            ManagerEmails = {managerEmails}
        };
    }

    public override async Task<GetStatisticsResponse> GetBikeReportStatistics(GetStatisticsRequest request, ServerCallContext context)
    {
        double totalReports = 0;
        double previousTotalReports = 0;
        var now = DateTime.UtcNow;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                totalReports = (await _unitOfWork.BikeReportRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfWeek.Date &&
                        x.CreatedOn.Date <= now.Date
                    )).Count();

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek);
                previousTotalReports = (await _unitOfWork.BikeReportRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn.Date <= filterDateOfPreviousWeek.Date
                    )).Count();
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalReports = (await _unitOfWork.BikeReportRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfMonth.Date &&
                        x.CreatedOn.Date <= now.Date
                    )).Count();

                var previousMonth = now.AddMonths(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalReports = (await _unitOfWork.BikeReportRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn.Date <= previousMonth.Date
                    )).Count();
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalReports = (await _unitOfWork.BikeReportRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfYear.Date &&
                        x.CreatedOn.Date <= now.Date
                    )).Count();

                var previousYear = now.AddYears(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalReports = (await _unitOfWork.BikeReportRepository
                    .Find(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn.Date <= previousYear.Date
                    )).Count();
                break;
        }
        
        double rateCompare;
        if (previousTotalReports == 0)
        {
            rateCompare = totalReports == 0 ? 0 : 100;
        }
        else
        {
            rateCompare = totalReports / previousTotalReports * 100 - 100;
        }

        return new GetStatisticsResponse
        {
            RateCompare = rateCompare,
            Total = totalReports
        };
    }
}
