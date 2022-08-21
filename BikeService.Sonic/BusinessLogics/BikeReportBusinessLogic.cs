using System.Linq.Expressions;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services;
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
        string? imageUrl = null;
        if (!string.IsNullOrEmpty(bikeReportInsertDto.ImageBase64))
        {
            var fileName = $"report-{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}";
            imageUrl = await FileUploadService.UploadBase64Image(bikeReportInsertDto.ImageBase64, fileName);
        }

        await _unitOfWork.BikeReportRepository.Add(new BikeReport
        {
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
            BikeId = bikeReportInsertDto.BikeId,
            Status = BikeReportStatus.NoFix,
            ReportDescription = bikeReportInsertDto.ReportDescription,
            AccountPhoneNumber = $"+{accountEmail.Split("@")[0]}",
            ImageUrl = imageUrl,
            AccountEmail = accountEmail,
            Title = bikeReportInsertDto.Title,
            UpdatedOn = DateTime.UtcNow
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
            .OrderByDescending(x => x.UpdatedOn)
            .Select(x => new BikeReportRetriveDto
            {
                Id = x.Id,
                AccountPhoneNumber = phoneNumber,
                BikeCode = x.Bike.BikeCode,
                AssignTo = x.AssignTo == null ? null : x.AssignTo.Email,
                CompletedOn = x.CompletedOn,
                Status = x.Status,
                ReportDescription = x.ReportDescription,
                ReportOn = x.CreatedOn,
                ImageUrl = x.ImageUrl,
                Title = x.Title
            }).ToList();
    }

    public async Task UpdateReportStatus(MarkReportAsResolveDto markReportAsResolveDto)
    {
        var bikeReport = await _unitOfWork.BikeReportRepository.GetById(markReportAsResolveDto.BikeReportId);
        if (bikeReport is null) return;
        
        bikeReport.Status = markReportAsResolveDto.Status;
        bikeReport.UpdatedOn = DateTime.UtcNow;
        bikeReport.CompletedOn = markReportAsResolveDto.Status == BikeReportStatus.Fixed ? DateTime.UtcNow : null;
        bikeReport.AssignToId = markReportAsResolveDto.AssignToId;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<BikeReportRetriveDto>> GetAllBikeReports()
    {
        return (await _unitOfWork.BikeReportRepository
                .All())
            .OrderByDescending(x => x.UpdatedOn)
            .AsNoTracking()
            .Select(x => new BikeReportRetriveDto
            {
                Id = x.Id,
                AccountPhoneNumber = x.AccountPhoneNumber,
                BikeCode = x.Bike.BikeCode,
                AssignTo = x.AssignTo == null ? null : x.AssignTo.Email,
                CompletedOn = x.CompletedOn,
                Status = x.Status,
                ReportDescription = x.ReportDescription,
                ReportOn = x.CreatedOn,
                ImageUrl = x.ImageUrl,
                Title = x.Title
            }).ToList();
    }
}
