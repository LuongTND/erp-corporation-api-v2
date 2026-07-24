namespace Infrastructure;

[RegisterService(typeof(IBackgroundJobService))]
public class EmailBackgroundService(IEmailSender emailSender, ILogger<EmailBackgroundService> logger) : IBackgroundJobService
{
    public Task EnqueueEmailAsync(EmailContent email)
    {
        _ = Task.Run(async () =>
        {
            try { await emailSender.SendEmailAsync(email); }
            catch (Exception ex) { logger.LogError(ex, "Email send failed to {Receiver}", email.Receiver); }
        });
        return Task.CompletedTask;
    }
}
