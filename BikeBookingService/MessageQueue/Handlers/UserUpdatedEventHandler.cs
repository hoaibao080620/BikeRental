// using BikeRental.MessageQueue.Events;
// using BikeRental.MessageQueue.Handlers;
// using BikeBookingService.DAL;
// using Newtonsoft.Json;
//
// namespace BikeBookingService.MessageQueue.Handlers;
//
// public class UserUpdatedEventHandler : IMessageQueueHandler
// {
//     private readonly IUnitOfWork _unitOfWork;
//
//     public UserUpdatedEventHandler(IUnitOfWork unitOfWork)
//     {
//         _unitOfWork = unitOfWork;
//     }
//     
//     public async Task Handle(string message)
//     {
//         var userUpdatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);
//         if(userUpdatedMessage is null) return;
//
//         var account = (await _unitOfWork.AccountRepository
//             .Find(x => x.ExternalId == userUpdatedMessage.Id)).FirstOrDefault();
//         
//         if(account is null) return;
//         
//         account.PhoneNumber = userUpdatedMessage.
//     }
// }
