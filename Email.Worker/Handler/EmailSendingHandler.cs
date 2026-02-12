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

    public async Task HandleAsync(IEmailContract message)
    {
        var model = EmailMapper.ToModel(message);

        Console.WriteLine($"📨 Enviando email para {model.ToEmail}");

        await _mailSendingService.SendEmailAsync(model);
    }
}
