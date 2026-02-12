using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Contracts;

public class MessageEnvelope<T>
{
    public string MessageType { get; set; } = default;
    public int Version { get;set; }
    public DateTime OcurredAt { get; set; } = DateTime.UtcNow;
    public T Payload { get; set; } = default;
}
