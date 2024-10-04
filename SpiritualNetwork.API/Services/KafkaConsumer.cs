using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using SpiritualNetwork.API.Model;
using SpiritualNetwork.API.Services.Interface;
using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Services
{
	public class KafkaConsumerBackgroundService : BackgroundService
	{
		private readonly List<string> _topics = new List<string> { "like","comment","post" };
		private readonly IServiceScopeFactory _serviceScopeFactory;

		public KafkaConsumerBackgroundService(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

		}
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Yield(); // Ensures this runs as a background task

			var config = new ConsumerConfig
			{
				BootstrapServers = "kafka-3f4b1c5a-k4m2a.e.aivencloud.com:25290",
				GroupId = "your-consumer-group-id",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false, // Disable auto commit
				SecurityProtocol = SecurityProtocol.Ssl,
				SslCaLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "ca.pem"),  // Path to the CA certificate
				SslCertificateLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.cert"),
				SslKeyLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.key"),
				//SslKeyPassword = "your-key-password",  // Optional if the key is password-protected
			};

			using (var consumer = new ConsumerBuilder<Null, string>(config).Build())
			{
				consumer.Subscribe(_topics);
				try
				{
					while (!stoppingToken.IsCancellationRequested)
					{
						var consumeResult = consumer.Consume(stoppingToken);
						var messageObject = JsonConvert.DeserializeObject<dynamic>(consumeResult.Message.Value);
						using (var scope = _serviceScopeFactory.CreateScope())
						{
							var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

							if (messageObject.Topic == "like")
							{
								var reactionService = scope.ServiceProvider.GetRequiredService<IReactionService>();
								int postId = messageObject.PostId;
								int userUniqueId = messageObject.UserUniqueId;
								// Call ToggleLike with the deserialized values
								var result = await reactionService.ToggleLike(postId, userUniqueId);

								if (result != null && result.Success)
								{
									// Manually commit the offset after successful processing
									consumer.Commit(consumeResult);
								}
								if (result != null && result.Result != null)
								{
									// Manually commit the offset after successful processing
									var notification = JsonConvert.DeserializeObject<Notification>(JsonConvert.SerializeObject(result.Result));
									await notificationService.SendNotification(notification);
								}
							}
							if (messageObject.Topic == "post")
							{
								consumer.Commit(consumeResult);
								var postDataDto = JsonConvert.DeserializeObject<PostDataDto>(consumeResult.Message.Value);
								// Convert the dictionary back to IFormCollection if needed
								var formCollection = new FormCollection(
									postDataDto.FormFields.ToDictionary(
										k => k.Key,
										v => new StringValues(v.Value)
									)
								);

								var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
								// Call the InsertPost method with the correct parameters
								var result = await postService.InsertPost(postDataDto);
								if (result != null && result.Success)
								{
									// Manually commit the offset after successful processing
									consumer.Commit(consumeResult);
								}
							}
							consumer.Commit(consumeResult);

						}
						Console.WriteLine($"Consumed message '{consumeResult.Message.Value}' from topic '{consumeResult.Topic}'");

						// Process the consumed message (e.g., store it in a database)
					}
				}
				catch (ConsumeException ex)
				{
					Console.WriteLine($"Consume error: {ex.Error.Reason}");
				}
				catch (OperationCanceledException)
				{
					// This is expected when the service is stopping
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Unexpected error: {ex.Message}");
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
