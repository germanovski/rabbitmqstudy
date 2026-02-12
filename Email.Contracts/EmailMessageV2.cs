using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Contracts
{
    public class EmailMessageV2 : IEmailContract
    {
        public EmailAddress From { get; set; }
        public EmailAddress To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml => false;
    }
}
