using Email.Contracts;
using Email.Worker;
using MailKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

Console.WriteLine("📨 Email Worker iniciado...");

long messagesReceived = 0;
long messagesSuccess = 0;
long messagesRetry = 0;
long messagesDlq = 0;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();
const int maxRetries = 3;

MailSendingService service = new();

var queueArguments = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", "email.dlx"},
    { "x-dead-letter-routing-key", "email.send.dlq"}
};

await channel.QueueDeclareAsync(queue: "email.send", durable: true, exclusive: false, autoDelete: false, arguments: queueArguments);

await channel.ExchangeDeclareAsync(
    exchange: "email.dlx",
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);

await channel.QueueDeclareAsync(queue: "email.send.dlq", durable: true, exclusive: false, autoDelete: false);

await channel.QueueBindAsync(queue: "email.send.dlq", exchange: "email.dlx", routingKey: "email.send.dlq");

var retryQueryArguments = new Dictionary<string, object>
{
    {"x-message-ttl", 5000 },
    {"x-dead-letter-exchange", "" },
    {"x-dead-letter-routing-key", "email.send" }
};

await channel.QueueDeclareAsync(
    queue: "email.retry",
    durable: true, 
    exclusive: false,
    autoDelete: false,
    arguments: retryQueryArguments);

await channel.BasicQosAsync(0, 1, false);

var consumer = new AsyncEventingBasicConsumer(channel);

Interlocked.Increment(ref messagesReceived);

consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var messageId = eventArgs.BasicProperties.MessageId ?? "no-id";

    try
    {

        var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        var envelope = JsonSerializer.Deserialize<MessageEnvelope<JsonElement>>(json);

        if (envelope is null)
            throw new InvalidOperationException("Envelope inválido");

        if (envelope.MessageType != "email.send")
            throw new InvalidOperationException("Tipo de mensagem desconhecido");

        if (envelope.Version != 1)
            throw new InvalidOperationException("Versão não suportada");

        EmailMessageV1 email = envelope.Payload.Deserialize<EmailMessageV1>() ?? throw new InvalidOperationException("Payload inválido");

        Console.WriteLine($"✉️ Email recebido: {email.Body}");
        //throw new Exception("Falha simulada no envio de email");
        await service.SendEmailAsync(email);

        await ((AsyncEventingBasicConsumer)sender).Channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        Console.WriteLine("✅ Email enviado");
    }
    catch (Exception ex)
    {
        int retryCount = 0;
        if (eventArgs.BasicProperties.Headers != null &&
        eventArgs.BasicProperties.Headers.TryGetValue("x-retry-count", out var value))
        {
            retryCount = value switch
            {
                byte[] bytes => BitConverter.ToInt32(bytes, 0),
                int i => i,
                long l => (int)l,
                _ => 0
            };
        }

        retryCount++;

        if (retryCount <= maxRetries)
        {
            Console.WriteLine($"[Retry] messageId={messageId} retry={retryCount}/{maxRetries} queue=email.retry");

            var props = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object>{ { "x-retry-count", retryCount } },
            };

            Interlocked.Increment(ref messagesRetry);

            await channel.BasicPublishAsync(
                exchange: "",
                mandatory: false,
                routingKey: "email.retry",
                basicProperties: props,
                body: eventArgs.Body);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
            Interlocked.Increment(ref messagesSuccess);
        }
        else
        {
            Interlocked.Increment(ref messagesDlq);
            Console.WriteLine("☠️ Limite de retries atingido. Enviando para DLQ");

            await channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false
            );
        }
    }
};

await channel.BasicConsumeAsync(queue: "email.send", autoAck: false, consumer: consumer);

Console.WriteLine("🟢 Aguardando mensagens...");

Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(TimeSpan.FromSeconds(30));
        Console.WriteLine(
            $"[Metrics] received={messagesReceived} " +
            $"success={messagesSuccess} retry={messagesRetry} dlq={messagesDlq}"
        );
    }
});


await Task.Delay(Timeout.Infinite);