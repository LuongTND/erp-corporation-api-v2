namespace Contract;

public sealed class EmailContent
{
    public required string Receiver { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}
