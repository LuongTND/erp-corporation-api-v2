namespace Application;

public sealed class WorkflowTaskMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // AssignedToName, EntityType, EntityId resolve từ join — map thủ công trong handler
        config.NewConfig<WorkflowTask, WorkflowTaskResponse>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Ignore(dest => dest.EntityType)
            .Ignore(dest => dest.EntityId)
            .Ignore(dest => dest.AssignedToName);
    }
}
