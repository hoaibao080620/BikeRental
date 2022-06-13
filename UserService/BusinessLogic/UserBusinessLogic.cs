using AutoMapper;
using Shared.Consts;
using UserService.Clients;
using UserService.DataAccess;
using UserService.Dtos;
using UserService.Exceptions;
using UserService.ExternalServices;
using UserService.Models;

namespace UserService.BusinessLogic;

public class UserBusinessLogic : IUserBusinessLogic
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOktaClient _oktaClient;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly IMapper _mapper;

    public UserBusinessLogic(
        IUnitOfWork unitOfWork,
        IOktaClient oktaClient,
        IMessageQueuePublisher messageQueuePublisher,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _oktaClient = oktaClient;
        _messageQueuePublisher = messageQueuePublisher;
        _mapper = mapper;
    }

    public async Task SyncOktaUsers()
    {
        var isUsersAlreadySync = await _unitOfWork.UserRepository.Exists(_ => true);
        if (isUsersAlreadySync) return;

        var isRolesAlreadySync = await _unitOfWork.RoleRepository.Exists(_ => true);
        if (!isRolesAlreadySync) await SyncOktaGroups();

        var groups = await _unitOfWork.RoleRepository.All();

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
                    RoleId = group.Id,
                    IsActive = true,
                    PhoneNumber = oktaUser.Profile.MobilePhone
                };
                
                await _unitOfWork.UserRepository.Add(user);
                await _unitOfWork.SaveChangesAsync();
                await _messageQueuePublisher.PublishUserAddedEventToMessageQueue(
                    user
                );
            }
        }
    }

    public async Task<IEnumerable<UserRetrieveDto>> GetUsers()
    {
        var users = await _unitOfWork.UserRepository.All();
        return _mapper.Map<List<UserRetrieveDto>>(users);
    }

    public async Task<UserRetrieveDto?> GetUserById(int id)
    {
        var user = await _unitOfWork.UserRepository.GetById(id);
        if (user is null) throw new UserNotFoundException(id);
        
        return _mapper.Map<UserRetrieveDto>(user);
    }

    public async Task AddUser(UserInsertDto userInsertDto)
    {
        var user = _mapper.Map<User>(userInsertDto);
        user.IsActive = true;
        user.CreatedOn = DateTime.UtcNow;
        user.RoleId = userInsertDto.RoleId ?? 
                      (await _unitOfWork.RoleRepository.Find(r => r.OktaRoleId == OktaGroup.UserGroup)).FirstOrDefault()!.Id;
        
        await _unitOfWork.UserRepository.Add(user);
        var isAdded = await _unitOfWork.SaveChangesAsync() > 0;
        
        if (isAdded)
        {
            await _messageQueuePublisher.PublishUserAddedEventToMessageQueue(user);
            var oktaUserId = await AddUserToOkta(userInsertDto);
            user.OktaUserId = oktaUserId;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task UpdateUser(int userId, UserUpdateDto userUpdateDto)
    {
        var user = await _unitOfWork.UserRepository.GetById(userId);
        if(user is null) throw new UserNotFoundException(userId);
        
        _mapper.Map(userUpdateDto, user);
        user.UpdatedOn = DateTime.UtcNow;
        
        await _unitOfWork.UserRepository.Update(user);
        var isUpdated = await _unitOfWork.SaveChangesAsync() > 0;
        
        if (isUpdated)
        {
            await _messageQueuePublisher.PublishUserUpdatedEventToMessageQueue(user);
        }
    }

    public async Task DeleteUser(int id)
    {
        var user = await _unitOfWork.UserRepository.GetById(id);
        if (user is null) throw new UserNotFoundException(id);

        await _unitOfWork.UserRepository.Delete(user);
        var isDeleted = await _unitOfWork.SaveChangesAsync() > 0;

        if (isDeleted)
        {
            var tasks = new List<Task>
            {
                _messageQueuePublisher.PublishUserDeletedEventToMessageQueue(user),
                DeleteOktaUser(user.OktaUserId)
            };

            await Task.WhenAll(tasks);
        }
    }
    
    private async Task<string?> AddUserToOkta(UserInsertDto userInsertDto)
    {
        var group = userInsertDto.RoleId switch
        {
            UserRole.User => OktaGroup.UserGroup,
            UserRole.Manager => OktaGroup.ManagerGroup,
            UserRole.SysAdmin => OktaGroup.SysAdminGroup,
            _ => OktaGroup.EveryoneGroup
        };

        var oktaUserId =  await _oktaClient.AddUserToOkta(new OktaUserInsertDto
        {
            FirstName = userInsertDto.FirstName,
            LastName = userInsertDto.LastName,
            Email = userInsertDto.Email,
            Password = userInsertDto.Password,
            PhoneNumber = userInsertDto.PhoneNumber,
            GroupId = group
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
            await _unitOfWork.RoleRepository.Add(new Role
            {
                OktaRoleId = group.Id,
                CreatedOn = group.Created,
                Description = group.Profile.Description,
                Name = group.Profile.Name
            });
        }

        await _unitOfWork.SaveChangesAsync();
    }
}