namespace Application;

// Mỗi item trong danh sách: StepId và StepOrder mới muốn đặt
public sealed record ReorderWorkflowStepsCommand(
    Guid TemplateId,
    IReadOnlyList<StepOrderItem> Items
) : IRequest<Unit>;

public sealed record StepOrderItem(Guid StepId, int StepOrder);
