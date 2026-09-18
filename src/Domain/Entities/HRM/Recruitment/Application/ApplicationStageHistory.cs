namespace Domain;

public class ApplicationStageHistory : EntityBase<Guid>
{
    public Guid ApplicationId { get; set; }
    public Application? Application { get; set; }

    public ApplicationStage FromStage { get; set; }
    public ApplicationStage ToStage { get; set; }

    public Guid ChangedByUserId { get; set; }
    public User? ChangedBy { get; set; }

    public string? Note { get; set; }

    public DateTimeOffset ChangedAt { get; set; }
}
