namespace Contract;

public sealed class EmailOptions
{
    public required string SenderEmail { get; set; }
    public required string SenderName { get; set; }
    public required string SmtpHost { get; set; }
    public required int SmtpPort { get; set; }
    public required string AppPassword { get; set; }
}
