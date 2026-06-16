namespace Domain.Base;

public interface ICreationTracked
{
    Guid? CreatedBy { get; set; }
}
