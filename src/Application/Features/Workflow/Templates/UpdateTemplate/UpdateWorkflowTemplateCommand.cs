namespace Application;

public sealed record UpdateWorkflowTemplateCommand(
    Guid TemplateId,
    string? Name,                                   // null → không đổi tên
    WorkflowScopeType? ScopeType,                   // null → không đổi scope (ScopeEntityId cũng bị bỏ qua)
    Guid? ScopeEntityId,                            // chỉ đọc khi ScopeType != null
    IReadOnlyList<UpdateWorkflowStepItem>? Steps    // null → không chạm vào steps
) : IRequest<Unit>;

public sealed record UpdateWorkflowStepItem(
    Guid? Id,           // null → tạo mới; có giá trị → update bước đó
    int StepOrder,
    string StepName,
    WorkflowApproverType ApproverType,
    Guid? ApproverId
);
