namespace Infrastructure;

public class WorkflowTemplateConfiguration : AuditableEntityConfiguration<WorkflowTemplate, Guid>
{
    public override void Configure(EntityTypeBuilder<WorkflowTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkflowTemplates");

        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(t => t.IsActive).HasDefaultValue(true).IsRequired();
        builder.Property(t => t.ScopeType).IsRequired();
        builder.Property(t => t.ScopeEntityId).IsRequired(false);

        builder.HasIndex(t => new { t.EntityType, t.ScopeType, t.ScopeEntityId }).IsUnique();

        builder.HasMany(t => t.Steps)
            .WithOne(s => s.Template)
            .HasForeignKey(s => s.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
