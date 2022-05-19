using AccountService.Models;
using Shared.Repositories;

namespace AccountService.DataAccess.Interfaces;

public interface IAccountRepository : IRepositoryGeneric<Account>
{
    
}