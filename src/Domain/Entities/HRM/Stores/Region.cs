namespace Domain;

public class Region : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string PosRegionId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<Store> Stores { get; set; } = [];
    public ICollection<RegionHours> RegionHours { get; set; } = [];
}
