using Confluent.Kafka;
using HotChocolate.Subscriptions;
using Newtonsoft.Json;
using SpiritualNetwork.Entities;

namespace SpiritualNetwork.API.Services
{
	public class KafkaProducer
	{
		public static async Task ProduceMessage<T>(string topicName, T message)
		{ 
			var config = new ProducerConfig
			{
				BootstrapServers = "kafka-3f4b1c5a-k4m2a.e.aivencloud.com:25290",
				SecurityProtocol = SecurityProtocol.Ssl,
				SslCaLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "ca.pem"),  // Path to the CA certificate
				SslCertificateLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.cert"),
				SslKeyLocation = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "service.key"),
				//SslKeyPassword = "your-key-password",  // (Optional) If your key has a password
			};

			using (var producer = new ProducerBuilder<Null, string>(config).Build())
			{
				try
				{
					//var result = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = message });
					var jsonMessage = JsonConvert.SerializeObject(message);
					producer.Produce(topicName, new Message<Null, string> {  Value = jsonMessage },
					(deliveryReport) =>
					{
						if (deliveryReport.Error.Code != ErrorCode.NoError)
						{
							Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
						}
						else
						{
							Console.WriteLine($"Produced event to topic {topicName}: value = {message}");
						}
					});
					//Console.WriteLine($"Message sent to topic {result.Topic}, partition {result.Partition}, offset {result.Offset}");
				}
				catch (ProduceException<Null, string> e)
				{
					Console.WriteLine($"Error: {e.Error.Reason}");
				}
				finally
				{
					// Ensure all messages in the queue are delivered before closing
					producer.Flush(TimeSpan.FromSeconds(10));  // Adjust timeout as necessary
				}
			}
		}
	}
}
