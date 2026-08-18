namespace Infrastructure;

public class CounterConfiguration : AuditableEntityConfiguration<Counter, Guid>
{
    public override void Configure(EntityTypeBuilder<Counter> builder)
    {
        base.Configure(builder);

        builder.ToTable("Counters");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);

        builder.HasIndex(c => new { c.StoreId, c.Code }).IsUnique();

        builder.HasOne(c => c.Store)
            .WithMany(s => s.Counters)
            .HasForeignKey(c => c.StoreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
