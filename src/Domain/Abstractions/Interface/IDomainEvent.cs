using MediatR;

namespace Domain;

public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; }
}
