namespace Application;

public class ConversationDto
{
    public Guid Id { get; set; }
    public ConversationType ConversationType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsArchived { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<ConversationMemberDto> Members { get; set; } = [];
    public MessageDto? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}

public class ConversationMemberDto
{
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string EmployeeCode { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public RoleInConversation? RoleInConversation { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsMuted { get; set; }
    public bool IsActive { get; set; }
}

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationID { get; set; }
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? Content { get; set; }
    public MessageType MessageType { get; set; }
    public Guid? ParentMessageID { get; set; }
    public bool IsEdited { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }

    public List<MessageAttachmentDto> Attachments { get; set; } = [];
    public List<MessageReactionDto> Reactions { get; set; } = [];
    public Guid? LinkedTaskId { get; set; }
}

public class MessageAttachmentDto
{
    public Guid Id { get; set; }
    public Guid MessageID { get; set; }
    public string FileName { get; set; } = null!;
    public string FileURL { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public int FileSize { get; set; }
    public string? ThumbnailURL { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class MessageReactionDto
{
    public Guid Id { get; set; }
    public Guid MessageID { get; set; }
    public Guid UserID { get; set; }
    public string FullName { get; set; } = null!;
    public string ReactionType { get; set; } = null!;
}

public class CreateConversationRequest
{
    public ConversationType ConversationType { get; set; } = ConversationType.Group;
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPrivate { get; set; } = true;
    public List<Guid> MemberIds { get; set; } = [];
}

public class CreateMessageRequest
{
    public string? Content { get; set; }
    public MessageType MessageType { get; set; } = MessageType.Text;
    public Guid? ParentMessageID { get; set; }
    public List<MessageAttachmentInput> Attachments { get; set; } = [];
}

public class MessageAttachmentInput
{
    public string FileName { get; set; } = null!;
    public string FileURL { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public int FileSize { get; set; }
    public string? ThumbnailURL { get; set; }
}