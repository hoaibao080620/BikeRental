using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Shared.Service;
using UserService.BusinessLogic;
using UserService.Dtos;

namespace UserService.Services;

public class ImportUser : IImportService
{
    private readonly IUserBusinessLogic _userBusinessLogic;

    public ImportUser(IUserBusinessLogic userBusinessLogic)
    {
        _userBusinessLogic = userBusinessLogic;
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
        var row = 0;
    
        while (await csvReader.ReadAsync())
        {
            row++;
            if(row == 1) continue;
            var firstName = csvReader.GetField(0);
            var lastName = csvReader.GetField(1);
            var address = csvReader.GetField(2);
            var email = csvReader.GetField(3);
            var phoneNumber = csvReader.GetField(4);
            var dateOfBirth = csvReader.GetField(5);
            var roleName = csvReader.GetField(6);
            var password = csvReader.GetField(7);

            await _userBusinessLogic.AddUser(new UserInsertDto
            {
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                DateOfBirth = DateTime.Parse(dateOfBirth),
                Email = email,
                PhoneNumber = phoneNumber,
                RoleName = roleName,
                Password = password
            });
        }
    }
}
