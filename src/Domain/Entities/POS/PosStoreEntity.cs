namespace Domain;

public sealed class PosStoreEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public Guid? RegionId { get; set; }
}

public sealed class PosRegionEntity
{
    public Guid Id { get; set; }
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsActive { get; set; }
}
