using BikeService.Sonic.Dtos;

namespace BikeService.Sonic.Services.Interfaces;

public interface IElasticSearchService
{
    Task AddBikeStationToIndex(BikeStationSearchDto bikeStationSearchDto);
    Task UpdateBikeStationRecord(int id, BikeStationSearchDto bikeStationSearchDto);
    Task DeleteBikeStationRecord(int id);
    Task<List<BikeStationSearchDto>> SearchBikeStationRecord(string queryString);
}