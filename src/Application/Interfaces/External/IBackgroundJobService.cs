namespace Application;

public interface IBackgroundJobService
{
    Task EnqueueEmailAsync(EmailContent email);
}
