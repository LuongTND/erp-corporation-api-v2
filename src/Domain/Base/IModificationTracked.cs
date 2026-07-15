namespace Domain;
public interface IModificationTracked
{
    Guid? UpdatedBy { get; set; }
}
