using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email.Contracts;

public static class EmailMapper
{
    public static EmailModel ToModel(IEmailContract contract)
    {
        return new EmailModel
        {
            FromEmail = contract.From.Email,
            ToEmail = contract.To.Email,
            Body = contract.Body,
            FromName = contract.From.Name,
            ToName = contract.To.Name,
            IsHtml = contract.IsHtml,
            Subject = contract.Subject,
        };
    }
}
