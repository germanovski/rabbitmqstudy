using Email.Contracts;
using Email.Worker.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Email.Worker.Handler;

public class EmailSendingHandler
{
    private readonly MailSendingService _mailSendingService;

    public EmailSendingHandler(MailSendingService mailSendingService)
    {
        _mailSendingService = mailSendingService;
    }

    public async Task HandleAsync(MessageEnvelope<JsonElement> envelope)
    {
        var email = envelope.Payload.Deserialize<EmailMessageV1>();

        if (email is null)
            throw new InvalidOperationException("Payload inválido.");

        Console.WriteLine($"📨 Enviando email para {email.To}");

        await _mailSendingService.SendEmailAsync(email);
    }
}
