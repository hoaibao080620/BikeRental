// using BikeRental.MessageQueue.Events;
// using BikeRental.MessageQueue.Handlers;
// using BikeTrackingService.DAL;
// using Newtonsoft.Json;
//
// namespace BikeTrackingService.MessageQueue.Handlers;
//
// public class UserUpdatedEventHandler : IMessageQueueHandler
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     public UserUpdatedEventHandler(IUnitOfWork unitOfWork)
//     {
//         _unitOfWork = unitOfWork;
//         _mongoService = mongoService;
//     }
//     
//     public async Task Handle(string message)
//     {
//         var userUpdatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);
//         if(userUpdatedMessage is null) return;
//
//         var account = (await _mongoService
//             .FindAccounts(x => x.ExternalUserId == userUpdatedMessage.Id)).FirstOrDefault();
//         
//         if(account is null) return;
//         
//         var builder = Builders<Account>.Update
//             .Set(x => x.FirstName, userUpdatedMessage.FirstName)
//             .Set(x => x.LastName, userUpdatedMessage.LastName)
//             .Set(x => x.PhoneNumber, userUpdatedMessage.PhoneNumber)
//             .Set(x => x.UpdatedOn, DateTime.UtcNow);
//
//         await _mongoService.UpdateAccount(account.Id, builder);
//     }
// }
