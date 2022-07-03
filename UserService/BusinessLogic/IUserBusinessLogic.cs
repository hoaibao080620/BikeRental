using UserService.Dtos;
using UserService.Dtos.User;

namespace UserService.BusinessLogic;

public interface IUserBusinessLogic
{
    Task<List<UserRetrieveDto>> GetUsers(string email);
    Task<UserRetrieveDto?> GetUserById(string id);
    Task AddUser(UserInsertDto user);
    Task UpdateUser(string userId, UserUpdateDto user);
    Task DeleteUser(string id);
    public Task<UserProfileDto?> GetUserProfile(string email);
    public Task SyncOktaUsers();
}
