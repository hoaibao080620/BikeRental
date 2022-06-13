// using BikeRental.MessageQueue.Handlers;
//
// namespace BikeService.Sonic.MessageQueueHandlers;
//
// public class AccountCreatedEventHandler : IMessageQueueHandler
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     public AccountCreatedEventHandler(IUnitOfWork unitOfWork)
//     {
//         _unitOfWork = unitOfWork;
//     }
//     
//     public async Task Handle(string message)
//     {
//         var userCreatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);
//
//         if(userCreatedMessage is null) return;
//
//         var user = AddUser(userCreatedMessage);
//         
//         await _unitOfWork.AccountRepository.Add(new Account
//         {
//             AccountCode = Guid.NewGuid(),
//             Balance = 0,
//             CreatedOn = DateTime.UtcNow,
//             IsActive = true,
//             UserId = user.Id
//         });
//         
//         await _unitOfWork.SaveChangesAsync();
//     }
//
//     private async Task<User> AddUser(UserCreated userCreatedMessage)
//     {
//         var user = new User
//         {
//             ExternalId = userCreatedMessage.Id,
//             FirstName = userCreatedMessage.FirstName,
//             LastName = userCreatedMessage.LastName,
//             PhoneNumber = userCreatedMessage.PhoneNumber,
//             Email = userCreatedMessage.Email
//         };
//         
//         await _unitOfWork.UserRepository.Add(user);
//         await _unitOfWork.SaveChangesAsync();
//
//         return user;
//     }
//     
// }