namespace Infrastructure;

public class UserCustomFieldValueConfiguration : BaseEntityConfiguration<UserCustomFieldValue, Guid>
{
    public override void Configure(EntityTypeBuilder<UserCustomFieldValue> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserCustomFieldValues");

        // Composite unique constraint — one value per user per field
        builder.HasIndex(v => new { v.UserId, v.DefinitionId }).IsUnique();

        builder.Property(v => v.Value).IsRequired().HasMaxLength(2000);

        builder.HasOne(v => v.User)
            .WithMany(u => u.CustomFieldValues)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Definition)
            .WithMany(d => d.Values)
            .HasForeignKey(v => v.DefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
