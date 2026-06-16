namespace Domain.Base;

public interface IModificationTracked
{
    Guid? UpdatedBy { get; set; }
}
