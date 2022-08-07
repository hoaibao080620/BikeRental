using System.Linq.Expressions;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
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
        var manager = (await _unitOfWork.BikeStationManagerRepository
            .Find(x => x.BikeStation.Bikes.Any(b => x.Id == bikeReportInsertDto.BikeId))).FirstOrDefault();

        await _unitOfWork.BikeReportRepository.Add(new BikeReport
        {
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
            BikeId = bikeReportInsertDto.BikeId,
            Status = BikeReportStatus.NoFix,
            ReportDescription = bikeReportInsertDto.ReportDescription,
            AccountPhoneNumber = accountEmail.Split("@")[0],
            AssignToId = manager!.ManagerId
        });

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<BikeReportRetriveDto>> GetBikeReports(string email)
    {
        var phoneNumber = email.Split("@")[0];
        var manager = (await _unitOfWork.ManagerRepository.Find(x => x.Email == email)).FirstOrDefault();
        Expression<Func<BikeReport, bool>> expression;

        if (manager is not null)
        {
            expression = x => manager.IsSuperManager || x.AssignToId == manager.Id;
        }
        else
        {
            expression = x => x.AccountPhoneNumber == phoneNumber;
        }

        return (await _unitOfWork.BikeReportRepository
                .Find(expression))
            .AsNoTracking()
            .Select(x => new BikeReportRetriveDto
            {
                Id = x.Id,
                BikeId = x.BikeId,
                AccountPhoneNumber = phoneNumber,
                BikeLicensePlate = x.Bike.BikeCode,
                CompletedBy = x.AssignTo.Email,
                CompletedOn = x.CompletedOn,
                Status = x.Status,
                ReportDescription = x.ReportDescription,
                ReportOn = x.CreatedOn
            }).ToList();
    }

    public async Task UpdateReportStatus(MarkReportAsResolveDto markReportAsResolveDto)
    {
        var bikeReport = await _unitOfWork.BikeReportRepository.GetById(markReportAsResolveDto.BikeReportId);
        if (bikeReport is null) return;
        
        bikeReport.CompletedOn = DateTime.UtcNow;
        bikeReport.Status = markReportAsResolveDto.Status;
        bikeReport.UpdatedOn = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }
}
