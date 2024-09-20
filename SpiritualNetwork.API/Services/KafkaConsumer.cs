using Confluent.Kafka;

namespace SpiritualNetwork.API.Services
{
	public class KafkaConsumerBackgroundService : BackgroundService
	{
		private readonly string _topicName = "like";  // Your Kafka topic name

		
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var config = new ConsumerConfig
			{
				BootstrapServers = "kafka-3f4b1c5a-k4m2a.e.aivencloud.com:25290",
				GroupId = "your-consumer-group-id",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				SecurityProtocol = SecurityProtocol.Ssl,
				SslCaLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "ca.pem"),  // Path to the CA certificate
				SslCertificateLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.cert"),
				SslKeyLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.key"),
				//SslKeyPassword = "your-key-password",  // Optional if the key is password-protected
			};

			using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
			{
				consumer.Subscribe(_topicName);

				try
				{
					while (!stoppingToken.IsCancellationRequested)
					{
						var consumeResult = await Task.Run(() => consumer.Consume(stoppingToken), stoppingToken);
						Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' from topic '{consumeResult.Topic}'");

						// Process the consumed message (e.g., store it in a database)
					}
				}
				catch (OperationCanceledException)
				{
					// This is expected when the service is stopping
				}
				finally
				{
					consumer.Close();
				}
			}
		}
		
		//public static void ConsumeMessages(string topicName)
		//{
		//	var config = new ConsumerConfig
		//	{
		//		BootstrapServers = "kafka-3f4b1c5a-k4m2a.e.aivencloud.com:25290",
		//		GroupId = "test-consumer-group",
		//		AutoOffsetReset = AutoOffsetReset.Earliest,
		//		SecurityProtocol = SecurityProtocol.Ssl,
		//		SslCaLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "ca.pem"),  // Path to the CA certificate
		//		SslCertificateLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.cert"),
		//		SslKeyLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.key"),
		//		//SslKeyPassword = "your-key-password",  // (Optional) If your key has a password
		//	};

		//	using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
		//	{
		//		consumer.Subscribe(topicName);

		//		CancellationTokenSource cts = new CancellationTokenSource();
		//		Console.CancelKeyPress += (_, e) => {
		//			e.Cancel = true;
		//			cts.Cancel();
		//		};

		//		try
		//		{
		//			while (true)
		//			{
		//				var consumeResult = consumer.Consume(cts.Token);
		//				Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");
		//			}
		//		}
		//		catch (OperationCanceledException)
		//		{
		//			consumer.Close();
		//		}
		//	}
		//}
	}
}
