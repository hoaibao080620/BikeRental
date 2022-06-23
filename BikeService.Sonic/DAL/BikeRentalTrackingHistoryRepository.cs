﻿using BikeService.Sonic.BikeDbContext;
using BikeService.Sonic.Models;
using Shared.Repositories;

namespace BikeService.Sonic.DAL;

public class BikeRentalTrackingHistoryRepository : 
    RepositoryGeneric<BikeLocationTrackingHistory, BikeServiceDbContext>, 
    IBikeRentalTrackingHistoryRepository
{
    public BikeRentalTrackingHistoryRepository(BikeServiceDbContext context) : base(context)
    {
    }
}
