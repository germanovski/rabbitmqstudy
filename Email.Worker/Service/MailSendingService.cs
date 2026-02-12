using Email.Contracts;
using MailKit.Net.Smtp;
using MimeKit;

namespace Email.Worker.Service;

public class MailSendingService
{
    const int smtpPort = 1025;

    public async Task SendEmailAsync(EmailModel model)
    {
        var message = new MimeMessage();

        var from = new MailboxAddress(model.FromName, model.FromEmail);
        message.From.Add(from);

        var to = new MailboxAddress(model.ToName, model.ToEmail);
        message.To.Add(to);

        message.Subject = model.Subject;

        var bb = new BodyBuilder();
        bb.TextBody = model.Body;

        message.Body = bb.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("localhost", smtpPort);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
        Console.WriteLine("Email enviado!");
    }
}
