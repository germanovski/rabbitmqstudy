using Email.Contracts;
using Email.Worker.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Email.Worker.Messaging
{
    public class ConsumerRouter
    {
        private readonly EmailSendingHandler _emailSendingHandler;

        public ConsumerRouter(EmailSendingHandler emailSendingHandler)
        {
            _emailSendingHandler = emailSendingHandler;
        }

        public async Task RouteAsync(MessageEnvelope<JsonElement> envelope)
        {
            switch(envelope.MessageType) 
            {
                case "email.send":
                    await _emailSendingHandler.HandleAsync(envelope);
                    break;

                default:
                    Console.WriteLine($"Message Ttype desconhecido: ${envelope.MessageType}");
                    break;
            }
        }
    }
}
