using BikeService.Sonic.Const;
using BikeService.Sonic.Models;
using MongoDB.Driver;

namespace BikeService.Sonic.DAL;

public class BikeRepository : IBikeRepository
{
    private readonly IMongoCollection<Bike> _bikeCollection;

    public BikeRepository(IMongoDatabase mongoDatabase)
    {
        _bikeCollection = mongoDatabase.GetCollection<Bike>(CollectionName.BikeCollection);
    }
    
    public async Task<List<Bike>> GetAll()
    {
        return await _bikeCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Bike> GetById(string id)
    {
        return await _bikeCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task Add(Bike bike)
    {
        await _bikeCollection.InsertOneAsync(bike);
    }

    public async Task Update(string id, Bike bike)
    {
        await _bikeCollection.ReplaceOneAsync(id, bike);
    }

    public async Task Delete(string id)
    {
        await _bikeCollection.DeleteOneAsync(b => b.Id == id);
    }
}