namespace Infrastructure;

public class TaskTemplateConfiguration : AuditableEntityConfiguration<TaskTemplate, Guid>
{
    public override void Configure(EntityTypeBuilder<TaskTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("TaskTemplates");

        builder.Property(t => t.TemplateName).IsRequired().HasMaxLength(255);
        builder.Property(t => t.Description).HasMaxLength(1000);

        builder.HasOne(t => t.DefaultPriority)
            .WithMany()
            .HasForeignKey(t => t.DefaultPriorityId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
