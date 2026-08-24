namespace Domain;

public class UserLabel : EntityBase<Guid>
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid LabelId { get; set; }
    public Label? Label { get; set; }
}
