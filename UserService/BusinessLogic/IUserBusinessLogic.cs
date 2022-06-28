using UserService.Dtos;
using UserService.Dtos.User;

namespace UserService.BusinessLogic;

public interface IUserBusinessLogic
{
    // Task<IEnumerable<UserRetrieveDto>> GetUsers();
    // Task<UserRetrieveDto?> GetUserById(int id);
    // Task AddUser(UserInsertDto user);
    // Task UpdateUser(int userId, UserUpdateDto user);
    // Task DeleteUser(int id);
    public Task<UserProfileDto?> GetUserProfile(string email);
    public Task SyncOktaUsers();
}
