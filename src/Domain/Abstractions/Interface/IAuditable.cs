namespace Domain;

public interface IAuditable
{
    Guid? CreatedBy { get; set; }
    Guid? UpdatedBy { get; set; }
}
