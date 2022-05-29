// using System.Collections.Concurrent;
// using System.Text;
// using BikeService.Sonic.Dtos;
// using BikeService.Sonic.Extensions;
// using BikeService.Sonic.Models;
// using BikeService.Sonic.Services.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Caching.Distributed;
// using MongoDB.Driver;
// using Nest;
// using Newtonsoft.Json;
// using SharpCompress.Compressors;
// using SharpCompress.Compressors.Deflate;
//
// namespace BikeService.Sonic.Controllers;
//
// [ApiController]
// [Route("[controller]/[action]")]
// public class BikeStationController : ControllerBase
// {
//     private readonly IElasticClient _elasticClient;
//     private readonly IElasticSearchService _elasticSearchService;
//     private readonly IDistributedCache _distributedCache;
//     private readonly IMongoDatabase _mongoDatabase;
//
//     public BikeStationController(
//         IElasticClient elasticClient,
//         IElasticSearchService elasticSearchService,
//         IDistributedCache distributedCache,
//         IMongoDatabase mongoDatabase)
//     {
//         _elasticClient = elasticClient;
//         _elasticSearchService = elasticSearchService;
//         _distributedCache = distributedCache;
//         _mongoDatabase = mongoDatabase;
//     }
//
//     [HttpPost]
//     public async Task<IActionResult> CreateDummyData()
//     {
//         var list = new List<BikeStation>();
//         const string cacheKey = "customerList";
//         for (var i = 0; i < 500; i++)
//         {
//             var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new BikeStation
//             {
//                 Id = i
//             }));
//             
//             await _distributedCache.SetAsync($"{cacheKey} {i}", bytes);
//             
//             list.Add(new BikeStation
//             {
//                 Id = i + 1,
//                 Name = Guid.NewGuid().ToString()
//             });
//         }
//         
//         // var collection = _mongoDatabase.GetCollection<BikeStation>("Bike");
//         // await collection.InsertManyAsync(list);
//         //
//         // var stations = new List<BikeStationSearchDto>
//         // {
//         //     new BikeStationSearchDto
//         //     {
//         //         Id = 1,
//         //         NameNormalize = "Tram so 4 Pham Van Dong",
//         //         DescriptionNormalize = "Doi dien DHKT Ho Chi Minh",
//         //         AddressNormalize = "14 Pham Van Dong, Ho Chi Minh"
//         //     },
//         //     new BikeStationSearchDto
//         //     {
//         //         Id = 2,
//         //         NameNormalize = "Tram so 17 Pham Ngu Lao",
//         //         DescriptionNormalize = "Doi dien quan nuong ba Hang",
//         //         AddressNormalize = "156 Pham Ngu Lao, Ho Chi Minh"
//         //     }
//         // };
//         //
//         // foreach (var station in stations)
//         // {
//         //     await _elasticClient.IndexDocumentAsync(station);
//         // }
//
//         return Ok();
//     }
//
//     [HttpGet]
//     public async Task<IActionResult> GetRedisDataTest()
//     {
//         const string cacheKey = "customerList";
//         var customerList = new ConcurrentBag<BikeStation>();
//         
//         var tasks = new List<Task>();
//
//         for (var i = 0; i < 500; i++)
//         {
//             tasks.Add(AddToList($"{cacheKey} {i}", customerList));
//         }
//
//         await Task.WhenAll(tasks);
//         
//         return Ok(customerList);
//     }
//     
//     [HttpGet]
//     public async Task<IActionResult> GetRedisData()
//     {
//         const string cacheKey = "customerList";
//         var customerList = new List<BikeStation>();
//         
//         for (var i = 0; i < 500; i++)
//         {
//             var cache = await _distributedCache.GetAsync($"{cacheKey} {i}");
//             if(cache is null) continue;
//             var value = JsonConvert.DeserializeObject<BikeStation>(Encoding.UTF8.GetString(cache));
//             customerList.Add(value);
//         }
//
//         return Ok(customerList);
//     }
//     
//     [HttpGet]
//     public async Task<IActionResult> GetMongoDbData()
//     {
//         var collection = _mongoDatabase.GetCollection<BikeStation>("Bike");
//         var list = await collection.Find(x => true).ToListAsync();
//         
//         return Ok(list);
//     }
//
//     private async Task AddToList(string cacheKey, ConcurrentBag<BikeStation> bikeStations)
//     {
//         var cache = await _distributedCache.GetAsync(cacheKey);
//         var value = JsonConvert.DeserializeObject<BikeStation>(Encoding.UTF8.GetString(cache));
//         bikeStations.Add(value);
//     }
//     
//     [HttpGet]
//     public async Task<IActionResult> Search([FromQuery] string queryString)
//     {
//         return Ok(await _elasticSearchService.SearchBikeStationRecord(queryString));
//     }
// }