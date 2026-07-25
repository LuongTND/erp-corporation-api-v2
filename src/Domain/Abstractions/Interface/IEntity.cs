namespace Domain;

public interface IEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset? ModifiedAt { get; set; }
}
