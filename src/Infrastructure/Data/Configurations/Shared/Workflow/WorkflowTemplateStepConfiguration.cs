namespace Infrastructure;

public class WorkflowTemplateStepConfiguration : BaseEntityConfiguration<WorkflowTemplateStep, Guid>
{
    public override void Configure(EntityTypeBuilder<WorkflowTemplateStep> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkflowTemplateSteps");

        builder.Property(s => s.StepName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.StepOrder).IsRequired();
        builder.Property(s => s.ApproverType).IsRequired();
        builder.Property(s => s.ApproverId).IsRequired(false);
    }
}
