using BikeService.Sonic.Const;
using BikeService.Sonic.Models;
using MongoDB.Driver;

namespace BikeService.Sonic.DAL;

public class BikeStationRepository : IBikeStationRepository
{
    private readonly IMongoCollection<BikeStation> _bikeCollection;

    public BikeStationRepository(IMongoDatabase mongoDatabase)
    {
        _bikeCollection = mongoDatabase.GetCollection<BikeStation>(CollectionName.BikeStationCollection);
    }
    
    public async Task<List<BikeStation>> GetAll()
    {
        return await _bikeCollection.Find(_ => true).ToListAsync();
    }

    public async Task<BikeStation> GetById(string id)
    {
        return await _bikeCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task Add(BikeStation bike)
    {
        await _bikeCollection.InsertOneAsync(bike);
    }

    public async Task Update(string id, BikeStation bike)
    {
        await _bikeCollection.ReplaceOneAsync(id, bike);
    }

    public async Task Delete(string id)
    {
        await _bikeCollection.DeleteOneAsync(b => b.Id == id);
    }
}