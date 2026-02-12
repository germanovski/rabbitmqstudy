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
                case "email.send.V1":
                    var v1 = envelope.Payload.Deserialize<EmailMessageV1>();
                    await _emailSendingHandler.HandleAsync(v1!);
                    break;

                case "email.send.V2":
                    var v2 = envelope.Payload.Deserialize<EmailMessageV2>();
                    await _emailSendingHandler.HandleAsync(v2!);
                    break;

                default:
                    Console.WriteLine($"Message Ttype desconhecido: ${envelope.MessageType}");
                    break;
            }
        }
    }
}
