using RabbitMQ.Client;
using System.Text;

public class RabbitMQService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqps://imqoltfn:7_XuLsLhQSHUY3gL6edDGW_A08yjbyiZ@fly.rmq.cloudamqp.com/imqoltfn")
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        //_channel.QueueDeclare(queue: "newposts", durable: false, exclusive: false, autoDelete: false, arguments: null);
    }

    public void PublishMessage(string queueName, string message)
    {
        _channel.QueueDeclare(queue: queueName,
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "",
                              routingKey: queueName,
                              basicProperties: null,
                              body: body);

        Console.WriteLine($" [x] Sent {message}");
    }

    public void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: "newpost", basicProperties: null, body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
