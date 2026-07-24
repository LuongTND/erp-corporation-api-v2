namespace Domain;

public class MessageAttachment : EntityBase<Guid>
{
    public Guid MessageID { get; set; }
    public Message? Message { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string FileURL { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string? ThumbnailURL { get; set; }
}
