using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Infrastructure;

[RegisterService(typeof(IEmailSender), ServiceLifetime.Singleton)]
public sealed class EmailSender : IEmailSender
{
    private readonly EmailOptions _settings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailOptions> settings, ILogger<EmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailContent emailModel)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(emailModel.Receiver, emailModel.Receiver));
        message.Subject = emailModel.Subject;
        message.Body = new BodyBuilder { HtmlBody = emailModel.Body }.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.AppPassword);
            await client.SendAsync(message);
            _logger.LogDebug("Email sent to {Receiver}", emailModel.Receiver);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Receiver}", emailModel.Receiver);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
