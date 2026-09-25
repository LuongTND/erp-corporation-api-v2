namespace Infrastructure;

public class ApplicationStageHistoryConfiguration : BaseEntityConfiguration<ApplicationStageHistory, Guid>
{
    public override void Configure(EntityTypeBuilder<ApplicationStageHistory> builder)
    {
        base.Configure(builder);

        builder.ToTable("ApplicationStageHistories");

        builder.Property(h => h.FromStage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.ToStage)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.Note).HasMaxLength(1000);

        builder.Property(h => h.ChangedAt).IsRequired();

        builder.HasIndex(h => h.ApplicationId);

        builder.HasOne(h => h.ChangedBy)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
