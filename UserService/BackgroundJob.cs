using MessageQueue;
using MessageQueue.Consumer;

namespace UserService;

public class BackgroundJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public BackgroundJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Test(stoppingToken);
    }
    
    private async Task Test(CancellationToken cancellationToken )
    {
        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IConsumer>();
        while (!cancellationToken.IsCancellationRequested)
        {
            await service.ReceiveMessages("https://sqs.us-east-1.amazonaws.com/707918051951/user-queue");
        }
    }
}