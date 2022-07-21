using AccountService.BusinessLogic.Interfaces;
using AccountService.DataAccess;
using AccountService.Dto;

namespace AccountService.BusinessLogic.Implementation;

public class AccountBusinessLogic : IAccountBusinessLogic
{
    private readonly IMongoService _mongoService;

    public AccountBusinessLogic(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }

    public async Task<AccountProfileDto> GetAccountProfile(string accountEmail)
    {
        var account = (await _mongoService.FindAccounts(x => x.Email == accountEmail)).FirstOrDefault();

        return account is null
            ? new AccountProfileDto()
            : new AccountProfileDto
            {
                Id = account.ExternalUserId,
                Point = account.Point,
                FirstName = account.FirstName,
                LastName = account.LastName,
                IsActive = account.IsActive,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber
            };
    }
}
