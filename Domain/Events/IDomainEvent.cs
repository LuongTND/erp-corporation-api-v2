using MediatR;

namespace Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
