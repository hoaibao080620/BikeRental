using System.Globalization;
using AccountService.DataAccess;
using Google.Protobuf.WellKnownTypes;
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
        var now = DateTime.UtcNow;
        var totalCount = 0;
        switch (request.FilterType)
        {
            case "week":
                var dayOfWeek = now.DayOfWeek;
                var firstDateOfWeek = now.AddDays((int) dayOfWeek * -1);
                var payments = await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfWeek.Date &&
                        x.CreatedOn <= now);
                
                totalPayment = payments.Sum(x => x.Amount);
                totalCount = payments.Count;

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
                payments = await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfMonth.Date &&
                        x.CreatedOn <= now);
                
                totalPayment = payments.Sum(x => x.Amount);
                totalCount = payments.Count;

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
                payments = await _mongoService
                    .FindAccountTransactions(x =>
                        x.Status == "Success" &&
                        x.CreatedOn >= firstDateOfYear.Date &&
                        x.CreatedOn <= now);
                
                totalPayment = payments.Sum(x => x.Amount);
                totalCount = payments.Count;

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
            Total = totalPayment,
            TotalCount = totalCount
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
            }).ToList();

        return new GetRecentTransactionsResponse
        {
            Transactions = { recentTransactions }
        };
    }

    public override async Task<GetAccountInfoResponse> GetAccountInfo(GetAccountInfoRequest request, ServerCallContext context)
    {
        var account = (await _mongoService.FindAccounts(x => x.Email == request.Email)).FirstOrDefault();

        if (account is null) return new GetAccountInfoResponse();
        
        var response = new GetAccountInfoResponse
        {
            Id = account.ExternalUserId,
            Email = account.Email,
            PhoneNumber = account.PhoneNumber,
            FirstName = account.FirstName,
            LastName = account.LastName,
            Point = account.Point,
        };

        if (account.Address != null)
        {
            response.Address = account.Address;
        }
        
        if (account.DateOfBirth != null)
        {
            response.DateOfBirth = Timestamp.FromDateTime(account.DateOfBirth.Value);
        }

        return response;
    }

    public override async Task<GetPaymentChartResponse> GetPaymentChart(GetPaymentChartRequest request, ServerCallContext context)
    {
        var chartColumnNumberDict = new Dictionary<string, int>
        {
            {"week", 7},
            {"month", 4},
            {"year", 12}
        };
        var startDate = request.StartDate.ToDateTime();
        var endDate = request.EndDate.ToDateTime();
        var startWeek = ISOWeek.GetWeekOfYear(startDate);
        var numberOfCharts = chartColumnNumberDict[request.FilterType];
        var chartData = new List<int>();
        var bikeRentalBookings = (await _mongoService.FindAccountTransactions(x =>
                x.TransactionTime >= startDate &&
                x.TransactionTime <= endDate.AddDays(1).AddTicks(-1))).ToList();

        var statistic = request.FilterType switch
        {
            "year" => bikeRentalBookings.GroupBy(x => x.TransactionTime.Month)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Count()),
            "month" => bikeRentalBookings.GroupBy(x => ISOWeek.GetWeekOfYear(x.TransactionTime))
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Count()),
            "week" => bikeRentalBookings.GroupBy(x => x.TransactionTime.DayOfWeek)
                .OrderBy(x => x.Key)
                .ToDictionary(x => (int) x.Key, x => x.Count()),
            _ => new Dictionary<int, int>()
        };
        
        for (var i = 1; i <= numberOfCharts; i++)
        {
            chartData.Add(statistic.GetValueOrDefault(i, statistic.GetValueOrDefault(startWeek + i - 1)));
        }
        
        if (request.FilterType == "week")
        {
            chartData[6] = statistic.GetValueOrDefault(0);
        }
        
        return new GetPaymentChartResponse()
        {
            ChartData = {chartData}
        };
    }
}
