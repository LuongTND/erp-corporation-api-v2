using MediatR;

namespace Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}