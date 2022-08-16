namespace BikeService.Sonic.Const;

public class RedisCacheKey
{
    public const string ManagerBikeIds = "BikeIds:{0}";
    public const string SingleBike = "Bike:Id:{0}";
    public const string BikeStationNearMeCache = "BikeStationNearMe:{0}";
}
