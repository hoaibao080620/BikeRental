using System.Globalization;
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

    public override async Task<GetStatisticsResponse> GetPaymentStatistics(GetStatisticsRequest request, ServerCallContext context)
    {
        double totalPayment = 0;
        double previousTotalPayment = 0;
        var now = DateTime.Now;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                totalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn.Date >= firstDateOfWeek.Date &&
                        x.CreatedOn.Date <= now.Date)).Sum(x => x.Amount);

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek);
                previousTotalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn.Date >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn.Date <= filterDateOfPreviousWeek.Date)).Sum(x => x.Amount);
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn.Date >= firstDateOfMonth.Date &&
                        x.CreatedOn.Date <= now.Date)).Sum(x => x.Amount);

                var previousMonth = now.AddMonths(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn.Date >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn.Date <= previousMonth.Date)).Sum(x => x.Amount);
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn.Date >= firstDateOfYear.Date &&
                        x.CreatedOn.Date <= now.Date)).Sum(x => x.Amount);

                var previousYear = now.AddYears(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn.Date >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn.Date <= previousYear.Date)).Sum(x => x.Amount);
                break;
        }

        return new GetStatisticsResponse
        {
            RateCompare = previousTotalPayment == 0 ? 100 : totalPayment / previousTotalPayment * 100 - 100,
            Total = totalPayment
        };
    }

    public override async Task<GetStatisticsResponse> GetAccountStatistics(GetStatisticsRequest request, ServerCallContext context)
    {
        double totalAccounts = 0;
        double previousTotalAccount = 0;
        var now = DateTime.Now;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                totalAccounts = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn.Date >= firstDateOfWeek.Date &&
                        x.CreatedOn.Date <= now.Date)).Count;

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek);
                previousTotalAccount = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn.Date <= filterDateOfPreviousWeek.Date)).Count;
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalAccounts = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn.Date >= firstDateOfMonth.Date &&
                        x.CreatedOn.Date <= now.Date)).Count;

                var previousMonth = now.AddMonths(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalAccount = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn.Date <= previousMonth.Date)).Count;
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalAccounts = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn.Date >= firstDateOfYear.Date &&
                        x.CreatedOn.Date <= now.Date)).Count;

                var previousYear = now.AddYears(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalAccount = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn.Date >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn.Date <= previousYear.Date)).Count;
                break;
        }

        return new GetStatisticsResponse
        {
            RateCompare = previousTotalAccount == 0 ? 100 : totalAccounts / previousTotalAccount * 100 - 100,
            Total = totalAccounts
        };
    }

    public override async Task<GetRecentTransactionsResponse> GetRecentTransactions(GetRecentTransactionsRequest request, ServerCallContext context)
    {
        var recentTransactions = (await _mongoService
                .FindAccountTransactions(_ => true, request.NumberOfItem))
            .Select(x => new RecentTransaction
            {
                Content = $"Tài khoản với số điện thoại {x.AccountPhoneNumber} nạp {x.Amount} vnd!",
                Status = x.Status
            });

        return new GetRecentTransactionsResponse
        {
            Transactions = { recentTransactions }
        };
    }
}
