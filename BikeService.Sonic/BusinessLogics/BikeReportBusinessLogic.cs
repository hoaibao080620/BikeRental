using System.Linq.Expressions;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Models;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeReportBusinessLogic : IBikeReportBusinessLogic
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeReportBusinessLogic(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task CreateReport(BikeReportInsertDto bikeReportInsertDto, string accountEmail)
    {
        var account = (await _unitOfWork.AccountRepository.Find(x => x.Email == accountEmail)).First();
        var manager = (await _unitOfWork.BikeStationManagerRepository
            .Find(x => x.BikeStation.Bikes.Any(b => x.Id == bikeReportInsertDto.BikeId))).FirstOrDefault();

        await _unitOfWork.BikeReportRepository.Add(new BikeReport
        {
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
            BikeId = bikeReportInsertDto.BikeId,
            IsResolved = false,
            ReportDescription = bikeReportInsertDto.ReportDescription,
            AccountId = account.Id,
            ManagerId = manager!.ManagerId
        });

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<BikeReportRetriveDto>> GetBikeReports(string email)
    {
        var manager = (await _unitOfWork.ManagerRepository.Find(x => x.Email == email)).FirstOrDefault();
        Expression<Func<BikeReport, bool>> expression;

        if (manager is not null)
        {
            expression = x => manager.IsSuperManager || x.ManagerId == manager.Id;
        }
        else
        {
            expression = x => x.Account.Email == email;
        }

        return (await _unitOfWork.BikeReportRepository
                .Find(expression))
            .AsNoTracking()
            .Select(x => new BikeReportRetriveDto
            {
                Id = x.Id,
                BikeId = x.BikeId,
                AccountReport = x.Account.Email,
                BikeLicensePlate = x.Bike.LicensePlate,
                CompletedBy = x.CompletedBy.Email,
                CompletedOn = x.CompletedOn,
                IsResolved = x.IsResolved,
                ReportDescription = x.ReportDescription,
                ReportOn = x.CreatedOn
            }).ToList();
    }

    public async Task<List<BikeReportRetriveDto>> GetUserReport(string userEmail)
    {
        return (await _unitOfWork.BikeReportRepository
                .Find(x => x.Account.Email == userEmail))
            .AsNoTracking()
            .Select(x => new BikeReportRetriveDto
            {
                Id = x.Id,
                BikeId = x.BikeId,
                AccountReport = x.Account.Email,
                BikeLicensePlate = x.Bike.LicensePlate,
                CompletedBy = x.CompletedBy.Email,
                CompletedOn = x.CompletedOn,
                IsResolved = x.IsResolved,
                ReportDescription = x.ReportDescription,
                ReportOn = x.CreatedOn
            }).ToList();
    }
}
