using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using UserService.BusinessLogic;
using UserService.Clients;
using UserService.DataAccess;
using UserService.Dtos;
using UserService.ExternalServices;
using UserService.Models;

namespace UserService.Test.UnitTests;

[TestFixture]
public class UserBusinessLogicTests
{
    private readonly IUserBusinessLogic _userBusinessLogic;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IMessageQueuePublisher> _messageQueuePublisher;
    private readonly Mock<IOktaClient> _oktaClient;
    private readonly Mock<IMapper> _mapper;
    private const int UserId = 1;

    public UserBusinessLogicTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _messageQueuePublisher = new Mock<IMessageQueuePublisher>();
        _oktaClient = new Mock<IOktaClient>();
        _mapper = new Mock<IMapper>();
        _userBusinessLogic = new UserBusinessLogic(
            _unitOfWork.Object,
            _oktaClient.Object,
            _messageQueuePublisher.Object,
            _mapper.Object);
    }
    
    [SetUp]
    public void Setup()
    {
        var userRepository = new Mock<IUserRepository>();
        _unitOfWork.SetupGet(x => x.UserRepository).Returns(userRepository.Object);
    }

    [Test]
    public async Task GetUsers_ShouldReturnUsersCorrectly()
    {
        // Arrange
        var setupUsers = SetupUsers();
        _unitOfWork.Setup(x => x.UserRepository.All()).Returns(Task.FromResult(setupUsers));
        _mapper.Setup(x => x.Map<List<UserRetrieveDto>>(It.IsAny<IEnumerable<User>>()))
            .Returns(new List<UserRetrieveDto>
            {
                new()
                {
                    Id = setupUsers.FirstOrDefault()!.Id
                }
            });

        // Act
        var users = await _userBusinessLogic.GetUsers();
        users = users.ToList();
        
        // Assert
        Assert.AreEqual(users.Count(), setupUsers.Count);
        Assert.AreEqual(users.FirstOrDefault()?.Id, setupUsers.FirstOrDefault()?.Id);
    }
    
    [Test]
    public async Task AddUser_ShouldAddUserSuccessfully()
    {
        // Arrange
        var userInsertDto = new UserInsertDto();
        var userReturn = new User();
        _mapper.Setup(x => x.Map<User>(It.IsAny<UserInsertDto>()))
            .Returns(userReturn);

        // Act
        await _userBusinessLogic.AddUser(userInsertDto);

        // Assert
        _unitOfWork.Verify(x => x.SaveChangesAsync(), Times.AtLeastOnce);
        Assert.IsTrue(userReturn.IsActive);
    }
    
    [Test]
    public async Task AddUser_ShouldAddMessageToQueueAddAddUserToOkta_WhenAddSuccessful()
    {
        // Arrange
        var userInsertDto = new UserInsertDto();
        _mapper.Setup(x => x.Map<User>(It.IsAny<UserInsertDto>()))
            .Returns(new User());

        _unitOfWork.Setup(x => x.SaveChangesAsync()).Returns(Task.FromResult(1));

        // Act
        await _userBusinessLogic.AddUser(userInsertDto);

        // Assert
        _messageQueuePublisher.Verify(x => 
            x.PublishUserAddedEventToMessageQueue(It.IsAny<User>()), Times.Once);
        _oktaClient.Verify(x => x.AddUserToOkta(It.IsAny<OktaUserInsertDto>()), Times.Once);
    }

    private static List<User> SetupUsers()
    {
        return new List<User>
        {
            new()
            {
                Id = UserId
            }
        };
    }
}