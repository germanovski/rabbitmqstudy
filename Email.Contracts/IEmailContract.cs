using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Contracts
{
    public interface IEmailContract
    {
        public EmailAddress From { get; }
        public EmailAddress To { get; }
        public string Subject { get; }
        public string Body { get; }
        public bool IsHtml { get; }
    }
}
