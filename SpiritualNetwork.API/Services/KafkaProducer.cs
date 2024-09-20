using Confluent.Kafka;

namespace SpiritualNetwork.API.Services
{
	public class KafkaProducer
	{
		public static async Task ProduceMessage(string topicName, string message)
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
					var result = await producer.ProduceAsync(topicName, new Message<Null, string> { Value = message });
					Console.WriteLine($"Message sent to topic {result.Topic}, partition {result.Partition}, offset {result.Offset}");
				}
				catch (ProduceException<Null, string> e)
				{
					Console.WriteLine($"Error: {e.Error.Reason}");
				}
			}
		}
	}
}
