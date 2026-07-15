namespace Domain;
public interface ICreationTracked
{
    Guid? CreatedBy { get; set; }
}
