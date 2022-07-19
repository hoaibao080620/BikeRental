using System.Globalization;
using AccountService.DataAccess;
using Grpc.Core;
using Shared.Service;
using TimeZone = Shared.Consts.TimeZone;

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
        var now = DateTime.UtcNow;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                totalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfWeek.Date &&
                        x.CreatedOn <= now)
                    ).Sum(x => x.Amount);

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek).AddDays(1).AddTicks(-1);
                previousTotalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn <= filterDateOfPreviousWeek)
                    ).Sum(x => x.Amount);
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfMonth.Date &&
                        x.CreatedOn <= now)
                    ).Sum(x => x.Amount);

                var previousMonth = now.AddMonths(-1).AddDays(1).AddTicks(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn <= previousMonth)
                    ).Sum(x => x.Amount);
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfYear.Date &&
                        x.CreatedOn <= now)).Sum(x => x.Amount);

                var previousYear = now.AddYears(-1).AddDays(1).AddTicks(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalPayment = (await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn <= previousYear))
                    .Sum(x => x.Amount);
                break;
        }
        
        double rateCompare;
        if (previousTotalPayment == 0)
        {
            rateCompare = totalPayment == 0 ? 0 : 100;
        }
        else
        {
            rateCompare = totalPayment / previousTotalPayment * 100 - 100;
        }

        return new GetStatisticsResponse
        {
            RateCompare = rateCompare,
            Total = totalPayment
        };
    }

    public override async Task<GetStatisticsResponse> GetAccountStatistics(GetStatisticsRequest request, ServerCallContext context)
    {
        double totalAccounts = 0;
        double previousTotalAccount = 0;
        var now = DateTime.UtcNow;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek == DayOfWeek.Sunday ? 7 : (int) now.DayOfWeek;
                var firstDateOfWeek = now.Subtract(TimeSpan.FromDays(dayOfWeek - 1));
                totalAccounts = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn >= firstDateOfWeek.Date &&
                        x.CreatedOn <= now)).Count;

                var previousWeek = ISOWeek.GetWeekOfYear(now) - 1;
                var firstDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, DayOfWeek.Monday);
                var filterDateOfPreviousWeek = ISOWeek.ToDateTime(now.Year, previousWeek, dayOfWeek == 7 ? DayOfWeek.Sunday : (DayOfWeek) dayOfWeek)
                    .AddDays(1).AddTicks(-1);
                previousTotalAccount = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn >= firstDateOfPreviousWeek.Date &&
                        x.CreatedOn <= filterDateOfPreviousWeek)).Count;
                break;
            case "month":
                var firstDateOfMonth = new DateTime(now.Year, now.Month, 1);
                totalAccounts = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn >= firstDateOfMonth.Date &&
                        x.CreatedOn <= now)).Count;

                var previousMonth = now.AddMonths(-1).AddDays(1).AddTicks(-1);
                var firstDateOfPreviousMonth= new DateTime(previousMonth.Year, previousMonth.Month, 1);
                previousTotalAccount = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn >= firstDateOfPreviousMonth.Date &&
                        x.CreatedOn <= previousMonth)).Count;
                break;
            case "year":
                var firstDateOfYear = new DateTime(now.Year, 1, 1);
                totalAccounts = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn >= firstDateOfYear.Date &&
                        x.CreatedOn <= now)).Count;

                var previousYear = now.AddYears(-1).AddDays(1).AddTicks(-1);
                var firstDateOfPreviousYear= new DateTime(previousYear.Year, 1, 1);
                previousTotalAccount = (await _mongoService
                    .FindAccounts(x =>
                        x.CreatedOn >= firstDateOfPreviousYear.Date &&
                        x.CreatedOn <= previousYear)).Count;
                break;
        }

        double rateCompare;
        if (previousTotalAccount == 0)
        {
            rateCompare = totalAccounts == 0 ? 0 : 100;
        }
        else
        {
            rateCompare = totalAccounts / previousTotalAccount * 100 - 100;
        }

        return new GetStatisticsResponse
        {
            RateCompare = rateCompare,
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
