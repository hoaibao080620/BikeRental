using System.Globalization;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Shared.Service;

namespace BikeService.Sonic.Services.Implementation;

public class BikeCsvImportService : IImportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBikeBusinessLogic _bikeBusinessLogic;

    public BikeCsvImportService(IUnitOfWork unitOfWork, IBikeBusinessLogic bikeBusinessLogic)
    {
        _unitOfWork = unitOfWork;
        _bikeBusinessLogic = bikeBusinessLogic;
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
            
            var description = csvReader.GetField(0);
            var bikeStationName = csvReader.GetField(1);

            var isBikeStationHasNeverBeenRetrieved = !bikeStationDict.ContainsKey(bikeStationName);

            if (isBikeStationHasNeverBeenRetrieved)
            {
                var bikeStation = await _unitOfWork.BikeStationRepository.GetBikeStationByName(bikeStationName);
                bikeStationDict.Add(bikeStationName, bikeStation);
            }

            await _bikeBusinessLogic.AddBike(new BikeInsertDto
            {
                Description = string.IsNullOrEmpty(description) ? null : description,
                BikeStationId = bikeStationDict[bikeStationName]?.Id
            });
        }
    }
}
