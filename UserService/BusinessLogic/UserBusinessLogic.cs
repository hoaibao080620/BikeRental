using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using MongoDB.Driver;
using Shared.Consts;
using UserService.Clients;
using UserService.DataAccess;
using UserService.Dtos;
using UserService.Dtos.User;
using UserService.ExternalServices;
using UserService.Models;

namespace UserService.BusinessLogic;

public class UserBusinessLogic : IUserBusinessLogic
{
    private readonly IOktaClient _oktaClient;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly IMongoService _mongoService;

    public UserBusinessLogic(
        IOktaClient oktaClient,
        IMessageQueuePublisher messageQueuePublisher,
        IMongoService mongoService)
    {
        _oktaClient = oktaClient;
        _messageQueuePublisher = messageQueuePublisher;
        _mongoService = mongoService;
    }

    public async Task<List<UserRetrieveDto>> GetUsers(string email)
    {
        var users = await _mongoService.FindUser(x => x.Email != email);

        return users.Select(u => new UserRetrieveDto
        {
            Id = u.Id,
            RoleName = u.RoleName,
            Address = u.Address,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            DateOfBirth = u.DateOfBirth,
            IsActive = u.IsActive,
            CreatedOn = u.CreatedOn,
            UpdatedOn = u.UpdatedOn
        }).ToList();
    }

    public async Task<UserRetrieveDto?> GetUserById(string id)
    {
        var user = (await _mongoService.FindUser(x => x.Id == id)).FirstOrDefault();

        return user is null ? null : new UserRetrieveDto
        {
            Id = user.Id,
            RoleName = user.RoleName,
            Address = user.Address,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth
        };
    }

    public async Task AddUser(UserInsertDto user)
    {
        var userAdded = new User
        {
            RoleName = user.RoleName,
            Address = user.Address,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };
        
        await _mongoService.AddUser(userAdded);
        await _messageQueuePublisher.PublishUserAddedEventToMessageQueue(userAdded);
        var oktaUserId = await AddUserToOkta(new OktaUserInsertParam
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Address = user.Address,
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            Password = user.Password,
            PhoneNumber = user.PhoneNumber,
            RoleName = user.RoleName
        });

        var updateOktaUserBuilder = Builders<User>.Update.Set(x => x.OktaUserId, oktaUserId);
        await _mongoService.UpdateUser(userAdded.Id, updateOktaUserBuilder);
    }

    public async Task UpdateUser(string userId, UserUpdateDto user)
    {
        var userUpdated = (await _mongoService.FindUser(x => x.Id == userId)).First();

        var originalRole = userUpdated.RoleName;
        var builder = Builders<User>.Update
            .Set(x => x.FirstName, user.FirstName)
            .Set(x => x.LastName, user.LastName)
            .Set(x => x.RoleName, string.IsNullOrEmpty(user.RoleName) ? userUpdated.RoleName : user.RoleName)
            .Set(x => x.Address, user.Address)
            .Set(x => x.DateOfBirth, user.DateOfBirth)
            .Set(x => x.UpdatedOn, DateTime.UtcNow);
        
        await _mongoService.UpdateUser(userId, builder);
        
        var newUserUpdated = (await _mongoService.FindUser(x => x.Id == userId)).First();
        await _messageQueuePublisher.PublishUserUpdatedEventToMessageQueue(newUserUpdated, user.ImageBase64);

        if (!string.IsNullOrEmpty(user.Password))
        {
            await _oktaClient.UpdateOktaUserPassword(userUpdated.OktaUserId!, user.Password);
        }

        if (originalRole != newUserUpdated.RoleName)
        {
            await UpdateOktaUserGroup(originalRole, newUserUpdated.RoleName, newUserUpdated.OktaUserId!);
            await _messageQueuePublisher.PublishUserRoleUpdatedEvent(new UserRoleUpdated
            {
                OriginalRole = originalRole,
                Email = newUserUpdated.Email,
                UserId = newUserUpdated.Id,
                NewRole = newUserUpdated.RoleName,
                MessageType = MessageType.UserRoleUpdated,
                FirstName = newUserUpdated.FirstName,
                LastName = newUserUpdated.LastName,
                PhoneNumber = newUserUpdated.PhoneNumber,
            });
        }
    }

    public async Task DeleteUser(string id)
    {
        var user = (await _mongoService.FindUser(x => x.Id == id)).FirstOrDefault();

        if (user is null) return;
        await _mongoService.DeleteUser(user.Id);
        await DeleteOktaUser(user?.OktaUserId);
        await _messageQueuePublisher.PublishUserDeletedEventToMessageQueue(new User
        {
            Id = id
        });
    }

    public async Task<UserProfileDto?> GetUserProfile(string email)
    {
        var user = (await _mongoService.FindUser(u => u.Email == email)).FirstOrDefault();
        
        return user is not null
            ? new UserProfileDto
            {
                Id = user.Id,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Email = email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.RoleName,
                PhoneNumber = user.PhoneNumber
            }
            : null;
    }

    public async Task SyncOktaUsers()
    {
        var isUsersAlreadySync = (await _mongoService.FindUser(_ => true)).Any();
        if(isUsersAlreadySync) return;
        
        var groups = await _mongoService.GetRoles();
        if (!groups.Any()) await SyncOktaGroups();
        
        foreach (var group in groups)
        {
            var oktaUsers = await _oktaClient.GetOktaUserByGroup(group.OktaRoleId);
            foreach (var oktaUser in oktaUsers.Where(o => o.Status == "ACTIVE"))
            {
                var user = new User
                {
                    OktaUserId = oktaUser.Id,
                    FirstName = oktaUser.Profile.FirstName,
                    LastName = oktaUser.Profile.LastName,
                    CreatedOn = oktaUser.Created,
                    Email = oktaUser.Profile.Email,
                    RoleName = group.Name,
                    IsActive = true,
                    PhoneNumber = oktaUser.Profile.MobilePhone
                };

                await _mongoService.AddUser(user);
                await _messageQueuePublisher.PublishUserAddedEventToMessageQueue(user);
            }
        }
    }

    public async Task ForgetPassword(ForgetPasswordDto forgetPasswordDto)
    {
        var user = (await _mongoService
            .FindUser(x => x.PhoneNumber == forgetPasswordDto.PhoneNumber))
            .FirstOrDefault();

        if (user is null) return;
        await _oktaClient.UpdateOktaUserPassword(user.OktaUserId!, forgetPasswordDto.NewPassword);
    }

    public async Task SignUp(SignUpDto signUpDto)
    {
        var userAdded = new User
        {
            RoleName = UserRole.User,
            Address = signUpDto.Address,
            Email = $"{signUpDto.PhoneNumber.Replace("+", "")}@gmail.com",
            FirstName = signUpDto.FirstName,
            LastName = signUpDto.LastName,
            PhoneNumber = signUpDto.PhoneNumber,
            DateOfBirth = signUpDto.DateOfBirth,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };
        
        await _mongoService.AddUser(userAdded);
        await _messageQueuePublisher.PublishUserAddedEventToMessageQueue(userAdded);
        var oktaUserId = await AddUserToOkta(new OktaUserInsertParam
        {
            FirstName = signUpDto.FirstName,
            LastName = signUpDto.LastName,
            Address = signUpDto.Address,
            DateOfBirth = signUpDto.DateOfBirth,
            Email = $"{signUpDto.PhoneNumber.Replace("+", "")}@gmail.com",
            Password = signUpDto.Password,
            PhoneNumber = signUpDto.PhoneNumber,
            RoleName = UserRole.User
        });

        var updateOktaUserBuilder = Builders<User>.Update.Set(x => x.OktaUserId, oktaUserId);
        await _mongoService.UpdateUser(userAdded.Id, updateOktaUserBuilder);
    }

    public async Task DeactivateUser(ActivateUserDto dto)
    {
        var user = await _mongoService.FindUser(x => x.Id == dto.UserId);
        if (user.Any())
        {
            var updateBuilder = Builders<User>.Update.Set(x => x.IsActive, false);
            await _mongoService.UpdateUser(dto.UserId, updateBuilder);
            await _messageQueuePublisher.PublishUserDeactivatedEvent(new UserDeactivated
            {
                UserId = dto.UserId,
                MessageType = MessageType.AccountDeactivated
            });
            await _oktaClient.DeactivateOktaUser(user.First().OktaUserId!);
        }
    }

    public async Task ActivateUser(ActivateUserDto dto)
    {
        var user = await _mongoService.FindUser(x => x.Id == dto.UserId);
        if (user.Any())
        {
            var updateBuilder = Builders<User>.Update.Set(x => x.IsActive, true);
            await _mongoService.UpdateUser(dto.UserId, updateBuilder);
            await _messageQueuePublisher.PublishUserActivatedEvent(new UserReactivated
            {
                UserId = dto.UserId,
                MessageType = MessageType.AccountReactivated
            });
            await _oktaClient.ActivateOktaUser(user.First().OktaUserId!);
        }
    }

    public async Task<List<UserRetrieveDto>> GetManagers()
    {
        var managers = await _mongoService.FindUser(x => x.RoleName == UserRole.Manager);
        return managers.Select(u => new UserRetrieveDto
        {
            Id = u.Id,
            RoleName = u.RoleName,
            Address = u.Address,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            DateOfBirth = u.DateOfBirth,
            IsActive = u.IsActive,
            CreatedOn = u.CreatedOn,
            UpdatedOn = u.UpdatedOn
        }).ToList();
    }

    public async Task ChangePassword(string email, ChangePasswordDto changePasswordDto)
    {
        var isOldPasswordCorrect = await _oktaClient.IsPasswordValid(email, changePasswordDto.OldPassword);
        if (!isOldPasswordCorrect) throw new InvalidOperationException();
        
        var user = (await _mongoService
                .FindUser(x => x.Email == email))
            .FirstOrDefault();

        if (user is null) return;
        await _oktaClient.UpdateOktaUserPassword(user.OktaUserId!, changePasswordDto.NewPassword);
    }

    public async Task SelfDelete(string email)
    {
        var user = (await _mongoService.FindUser(x => x.Email == email)).First();
        await _mongoService.DeleteUser(user.Id);
        await DeleteOktaUser(user.OktaUserId);
        await _messageQueuePublisher.PublishUserDeletedEventToMessageQueue(new User
        {
            Id = user.Id
        });
    }

    private async Task<string?> AddUserToOkta(OktaUserInsertParam oktaInsertParam)
    {
        var group = (await _mongoService.GetRoles()).FirstOrDefault(x => x.Name == oktaInsertParam.RoleName)!;
    
        var oktaUserId =  await _oktaClient.AddUserToOkta(new OktaUserInsertDto
        {
            FirstName = oktaInsertParam.FirstName,
            LastName = oktaInsertParam.LastName,
            Email = oktaInsertParam.Email,
            PhoneNumber = oktaInsertParam.PhoneNumber,
            GroupId = group.OktaRoleId,
            Password = oktaInsertParam.Password
        });
    
        return oktaUserId;
    }
    
    private async Task DeleteOktaUser(string? oktaUserId)
    {
        await _oktaClient.DeleteOktaUser(oktaUserId);
    }

    private async Task SyncOktaGroups()
    {
        var groups = await _oktaClient.GetOktaGroups();
        foreach (var group in groups)
        {
            await _mongoService.AddRole(new Role
            {
                OktaRoleId = group.Id,
                CreatedOn = group.Created,
                Description = group.Profile.Description,
                Name = group.Profile.Name
            });
        }
    }
    
    private async Task UpdateOktaUserGroup(string originalRoleName, string newRoleName, string oktaUserId)
    {
        var originalGroup = (await _mongoService.GetRoles()).FirstOrDefault(x => x.Name == originalRoleName)!;
        var newGroup = (await _mongoService.GetRoles()).FirstOrDefault(x => x.Name == newRoleName)!;
        await _oktaClient.UpdateOktaUserRole(originalGroup.OktaRoleId, newGroup.OktaRoleId, oktaUserId);
    }
}
