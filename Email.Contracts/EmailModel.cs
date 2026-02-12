using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Contracts
{
    public class EmailModel
    {
        public string FromName { get; set; } = default!;
        public string FromEmail { get; set; } = default!;
        public string ToName { get; set; } = default!;
        public string ToEmail { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public bool IsHtml { get; set; }
    }
}
