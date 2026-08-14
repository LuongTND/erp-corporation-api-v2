namespace Domain;

public class Store : AuditableEntityBase<Guid>, ISoftDeletable
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string PosStoreId { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }

    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public ICollection<StoreHours> StoreHours { get; set; } = [];
    public ICollection<Counter> Counters { get; set; } = [];
    public ICollection<UserStore> UserStores { get; set; } = [];
}
