using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailWithRabbitmq
{
    public class MailSendingService
    {
        const int smtpPort = 1025;
        private RabbitmqPublisher publisher = new RabbitmqPublisher();

        public async void SendEmail(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            
            var from = new MailboxAddress(emailMessage.From.Name, emailMessage.From.Email);
            message.From.Add(from);

            var to = new MailboxAddress(emailMessage.To.Name, emailMessage.To.Email);
            message.To.Add(to);

            message.Subject = emailMessage.Subject;

            var bb = new BodyBuilder();
            bb.TextBody = emailMessage.Body;

            message.Body = bb.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("localhost", smtpPort);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
            Console.WriteLine("Email enviado!");

            //publisher.Publish(emailMessage.Body, emailMessage.From.Name, emailMessage.To.Name);
        }
    }
}
