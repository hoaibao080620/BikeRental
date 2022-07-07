﻿using BikeService.Sonic.Dtos.Bike;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeReportBusinessLogic
{
    Task CreateReport(BikeReportInsertDto bikeReportInsertDto, string accountEmail);
    Task<List<BikeReportRetriveDto>> GetBikeReports(string email); }
