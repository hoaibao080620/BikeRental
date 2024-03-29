﻿using UserService.Dtos;
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
    public Task ForgetPassword(ForgetPasswordDto forgetPasswordDto);
    public Task SignUp(SignUpDto signUpDto);
    public Task DeactivateUser(ActivateUserDto dto);
    public Task ActivateUser(ActivateUserDto dto);
    Task<List<UserRetrieveDto>> GetManagers();
    public Task ChangePassword(string email, ChangePasswordDto changePasswordDto);
    public Task SelfDelete(string email);
}
