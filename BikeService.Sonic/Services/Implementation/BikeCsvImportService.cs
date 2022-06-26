using System.Globalization;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Shared.Service;

namespace BikeService.Sonic.Services.Implementation;

public class BikeCsvImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeCsvImportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Import(IFormFile formFile)
    {
        if (Path.GetExtension(formFile.FileName) != ".csv") throw new InvalidOperationException("File upload not csv");
        
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            Comment = '#',
            AllowComments = true,
            Delimiter = ","
        };
        
        await using var stream = formFile.OpenReadStream();
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, csvConfig);
        var bikeStationDict = new Dictionary<string, BikeStation?>();
        var row = 0;
    
        while (await csvReader.ReadAsync())
        {
            row++;
            if(row == 1) continue;
            
            var licensePlate = csvReader.GetField(0);
            var description = csvReader.GetField(1);
            var bikeStationName = csvReader.GetField(2);

            var isBikeStationHasNeverBeenRetrieved = !bikeStationDict.ContainsKey(bikeStationName);

            if (isBikeStationHasNeverBeenRetrieved)
            {
                var bikeStation = await _unitOfWork.BikeStationRepository.GetBikeStationByName(bikeStationName);
                bikeStationDict.Add(bikeStationName, bikeStation);
            }

            await _unitOfWork.BikeRepository.Add(new Bike
            {
                LicensePlate = licensePlate,
                Description = string.IsNullOrEmpty(description) ? null : description,
                BikeStationId = bikeStationDict[bikeStationName]?.Id,
                IsActive = true,
                CreatedOn = DateTime.UtcNow,
                Status = BikeStatus.Available
            });
        }
        
        await _unitOfWork.SaveChangesAsync();
    }
}
