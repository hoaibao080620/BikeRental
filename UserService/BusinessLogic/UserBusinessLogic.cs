using AutoMapper;
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
    private readonly IMapper _mapper;

    public UserBusinessLogic(
        IOktaClient oktaClient,
        IMessageQueuePublisher messageQueuePublisher,
        IMongoService mongoService,
        IMapper mapper)
    {
        _oktaClient = oktaClient;
        _messageQueuePublisher = messageQueuePublisher;
        _mongoService = mongoService;
        _mapper = mapper;
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
            DateOfBirth = u.DateOfBirth
        }).ToList();
    }

    public async Task<UserRetrieveDto?> GetUserById(string id)
    {
        var user = (await _mongoService.FindUser(x => x.Id == id)).FirstOrDefault();

        return user is null ? null : new UserRetrieveDto()
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
            RoleName = user.RoleName ?? "Users",
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
        var oktaUserId = await AddUserToOkta(user);

        var updateOktaUserBuilder = Builders<User>.Update.Set(x => x.OktaUserId, oktaUserId);
        await _mongoService.UpdateUser(userAdded.Id, updateOktaUserBuilder);
    }

    public async Task UpdateUser(string userId, UserUpdateDto user)
    {
        var builder = Builders<User>.Update
            .Set(x => x.FirstName, user.FirstName)
            .Set(x => x.LastName, user.LastName)
            .Set(x => x.RoleName, user.RoleName)
            .Set(x => x.Address, user.Address)
            .Set(x => x.PhoneNumber, user.PhoneNumber)
            .Set(x => x.DateOfBirth, user.DateOfBirth)
            .Set(x => x.UpdatedOn, DateTime.UtcNow);
        
        await _mongoService.UpdateUser(userId, builder);
    }

    public async Task DeleteUser(string id)
    {
        await _mongoService.DeleteUser(id);
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

    // public async Task<IEnumerable<UserRetrieveDto>> GetUsers()
    // {
    //     var users = await _unitOfWork.UserRepository.All();
    //     return _mapper.Map<List<UserRetrieveDto>>(users);
    // }
    //
    // public async Task<UserRetrieveDto?> GetUserById(int id)
    // {
    //     var user = await _unitOfWork.UserRepository.GetById(id);
    //     if (user is null) throw new UserNotFoundException(id);
    //     
    //     return _mapper.Map<UserRetrieveDto>(user);
    // }
    

    // public async Task UpdateUser(int userId, UserUpdateDto userUpdateDto)
    // {
    //     var user = await _unitOfWork.UserRepository.GetById(userId);
    //     if(user is null) throw new UserNotFoundException(userId);
    //     
    //     _mapper.Map(userUpdateDto, user);
    //     user.UpdatedOn = DateTime.UtcNow;
    //     
    //     await _unitOfWork.UserRepository.Update(user);
    //     var isUpdated = await _unitOfWork.SaveChangesAsync() > 0;
    //     
    //     if (isUpdated)
    //     {
    //         await _messageQueuePublisher.PublishUserUpdatedEventToMessageQueue(user);
    //     }
    // }

    // public async Task DeleteUser(int id)
    // {
    //     var user = await _unitOfWork.UserRepository.GetById(id);
    //     if (user is null) throw new UserNotFoundException(id);
    //
    //     await _unitOfWork.UserRepository.Delete(user);
    //     var isDeleted = await _unitOfWork.SaveChangesAsync() > 0;
    //
    //     if (isDeleted)
    //     {
    //         var tasks = new List<Task>
    //         {
    //             _messageQueuePublisher.PublishUserDeletedEventToMessageQueue(user),
    //             DeleteOktaUser(user.OktaUserId)
    //         };
    //
    //         await Task.WhenAll(tasks);
    //     }
    // }
    
    private async Task<string?> AddUserToOkta(UserInsertDto userInsertDto)
    {
        var group = (await _mongoService.GetRoles()).FirstOrDefault(x => x.Name == userInsertDto.RoleName)!;
    
        var oktaUserId =  await _oktaClient.AddUserToOkta(new OktaUserInsertDto
        {
            FirstName = userInsertDto.FirstName,
            LastName = userInsertDto.LastName,
            Email = userInsertDto.Email,
            Password = userInsertDto.Password,
            PhoneNumber = userInsertDto.PhoneNumber,
            GroupId = group.OktaRoleId
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
}
