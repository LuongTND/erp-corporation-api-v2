namespace Domain;

public class Label : EntityBase<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6B7280";
    public bool IsActive { get; set; } = true;

    public ICollection<UserLabel> UserLabels { get; set; } = [];
}
