namespace Domain;

public class MessageAttachment : BaseEntity
{
    public Guid MessageID { get; private set; }
    public virtual Message Message { get; private set; } = null!;

    public string FileName { get; private set; } = null!;
    public string FileURL { get; private set; } = null!;
    public string FileType { get; private set; } = null!;
    public int FileSize { get; private set; }
    public string? ThumbnailURL { get; private set; }

    private MessageAttachment() : base()
    {
    }

    public static MessageAttachment Create(
        Guid messageId,
        string fileName,
        string fileUrl,
        string fileType,
        int fileSize,
        string? thumbnailUrl = null)
    {
        return new MessageAttachment
        {
            MessageID = messageId,
            FileName = fileName.Trim(),
            FileURL = fileUrl.Trim(),
            FileType = fileType.Trim().ToLowerInvariant(),
            FileSize = fileSize,
            ThumbnailURL = thumbnailUrl?.Trim()
        };
    }
}