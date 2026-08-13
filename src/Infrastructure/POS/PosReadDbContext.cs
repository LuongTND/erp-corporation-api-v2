namespace Infrastructure;

public sealed class PosReadDbContext(DbContextOptions<PosReadDbContext> options) : DbContext(options)
{
    public DbSet<PosStoreEntity> Stores => Set<PosStoreEntity>();
    public DbSet<PosRegionEntity> Regions => Set<PosRegionEntity>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<PosStoreEntity>().ToTable("Stores").HasKey(s => s.Id);
        mb.Entity<PosRegionEntity>().ToTable("Regions").HasKey(r => r.Id);
    }
}
