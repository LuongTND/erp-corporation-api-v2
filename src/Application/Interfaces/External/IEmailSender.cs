namespace Application;

public interface IEmailSender
{
    Task SendEmailAsync(EmailContent email);
}
