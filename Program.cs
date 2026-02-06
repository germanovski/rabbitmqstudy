using MailWithRabbitmq;

class Program
{
    static async Task Main(string[] args)
    {
        var publisher = new RabbitmqPublisher();

        var from = new EmailAddress { Name = "Matheus Germano", Email = "math@gmail.com" };
        var to = new EmailAddress { Name = "Artemísia Germano", Email = "artemisia@gmail.com" };

        EmailMessage message = new EmailMessage(from, to, "Email de teste", "Esta é uma mensagem de testes.");

        await publisher.Publish(message.Body, message.From.Name, message.To.Name);
    }
}