using AccountService.Dto;

namespace AccountService.BusinessLogic.Interfaces;

public interface IAccountBusinessLogic
{
    Task<AccountProfileDto> GetAccountProfile(string accountEmail);
}
