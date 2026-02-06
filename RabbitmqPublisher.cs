using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MailWithRabbitmq;

public class RabbitmqPublisher
{
    public async Task Publish(string message, string senderName, string recipientName)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "email.send", durable: true, exclusive: false, autoDelete: false);

        //var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "email.send", mandatory: false,
                                                   basicProperties: new BasicProperties { Persistent = true },
                                                   body: body);

        Console.WriteLine($"{senderName} enviou um email para {recipientName}.");
    }
}
