using AccountService.DataAccess;
using Grpc.Core;

namespace AccountService.GrpcServices;

public class AccountGrpcService : AccountServiceGrpc.AccountServiceGrpcBase
{
    private readonly IMongoService _mongoService;

    public AccountGrpcService(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }

    public override async Task<GetAccountPointResponse> GetAccountPoint(GetAccountPointRequest request, ServerCallContext context)
    {
        var account = (await _mongoService.FindAccounts(x => x.Email == request.AccountEmail)).FirstOrDefault();

        return new GetAccountPointResponse
        {
            Point = account?.Point ?? default
        };
    }
}
