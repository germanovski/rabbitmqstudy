using Email.Contracts;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Email.API.Messaging
{
    public class RabbitmqEmailProducer
    {
        private readonly ConnectionFactory _factory;

        public RabbitmqEmailProducer()
        {
            _factory = new ConnectionFactory { HostName = "localhost"};
        }

        public async Task PublishAsync(EmailMessage message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var props = new BasicProperties
            {
                Persistent = true,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.Now.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "email.send",
                mandatory: false,
                basicProperties: props,
                body: body);

            Console.WriteLine("📤 Publicando mensagem no RabbitMQ");

        }
    }
}
