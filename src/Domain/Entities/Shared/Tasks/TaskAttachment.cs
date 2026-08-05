namespace Domain;

public class TaskAttachment : EntityBase<Guid>
{
    public Guid TaskId { get; set; }
    public TaskItem? Task { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Size { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string DataUrl { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
}
