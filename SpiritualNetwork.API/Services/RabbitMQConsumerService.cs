using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
public class RabbitMQConsumerService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public RabbitMQConsumerService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqps://imqoltfn:7_XuLsLhQSHUY3gL6edDGW_A08yjbyiZ@fly.rmq.cloudamqp.com/imqoltfn")
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _queueName = "newposts"; // Change to your queue name

        _channel.QueueDeclare(queue: _queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);
    }

    public void StartListening()
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received {message}");
            var userPost = JsonConvert.DeserializeObject<UploadPostResponse>(message);
            //await NotifyService(userPost);
            // Acknowledge message
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _queueName,
                             autoAck: false, // Set to false to manually acknowledge messages
                             consumer: consumer);

        Console.WriteLine("Consumer started. Press [enter] to exit.");
    }

    private async Task NotifyService(UploadPostResponse userPost)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();


            NotificationRes notification = new NotificationRes();
            notification.PostId = userPost.Post.Id;
            notification.ActionByUserId = userPost.Post.UserId;
            notification.ActionType = userPost.Post.Type;
            notification.RefId1 = userPost.Post.ParentId.ToString();
            notification.RefId2 = "";
            notification.Message = "";
            // Call the method on the scoped notification service
            await Task.Run(() => notificationService.SaveNotification(notification));
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
