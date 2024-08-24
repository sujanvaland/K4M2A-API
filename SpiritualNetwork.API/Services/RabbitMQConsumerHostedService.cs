using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQConsumerHostedService : BackgroundService
{
    private readonly RabbitMQConsumerService _consumerService;

    public RabbitMQConsumerHostedService(RabbitMQConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumerService.StartListening();
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumerService.Dispose();
        base.Dispose();
    }
}
