namespace Email.Contracts;

public class EmailMessageV1
{
    public EmailAddressV1 From { get; set; }
    public EmailAddressV1 To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public EmailMessageV1(EmailAddressV1 from, EmailAddressV1 to, string subject, string body)
    {
        From = from ?? throw new ArgumentNullException(nameof(from));
        To = to ?? throw new ArgumentNullException(nameof(to));
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Body = body ?? throw new ArgumentNullException(nameof(body));
    }
}
