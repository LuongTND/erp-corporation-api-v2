namespace Application;

public class CreateNotificationEventTypeRequestValidator : AbstractValidator<CreateNotificationEventTypeRequest>
{
    public CreateNotificationEventTypeRequestValidator()
    {
        RuleFor(x => x.EventCode).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Module).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DefaultTitleTemplate).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DefaultBodyTemplate).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class UpdateNotificationEventTypeRequestValidator : AbstractValidator<UpdateNotificationEventTypeRequest>
{
    public UpdateNotificationEventTypeRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Module).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DefaultTitleTemplate).NotEmpty().MaximumLength(500);
        RuleFor(x => x.DefaultBodyTemplate).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class
    UpdateNotificationTriggerBindingRequestValidator : AbstractValidator<UpdateNotificationTriggerBindingRequest>
{
    public UpdateNotificationTriggerBindingRequestValidator()
    {
        RuleFor(x => x.LinkUrlTemplate).MaximumLength(500);
    }
}

public class UpsertNotificationTemplateRequestValidator : AbstractValidator<UpsertNotificationTemplateRequest>
{
    public UpsertNotificationTemplateRequestValidator()
    {
        RuleFor(x => x.TitleTemplate).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BodyTemplate).NotEmpty();
    }
}