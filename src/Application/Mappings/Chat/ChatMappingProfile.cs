namespace Application;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<Conversation, ConversationDto>()
            .ForMember(d => d.Members, opt => opt.MapFrom(s => s.Members.Where(m => m.IsActive)))
            .ForMember(d => d.LastMessage, opt => opt.Ignore())
            .ForMember(d => d.UnreadCount, opt => opt.Ignore());

        CreateMap<ConversationMember, ConversationMemberDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty))
            .ForMember(d => d.EmployeeCode,
                opt => opt.MapFrom(s => s.User != null ? s.User.EmployeeCode : string.Empty))
            .ForMember(d => d.AvatarUrl, opt => opt.MapFrom(s => s.User != null ? s.User.AvatarUrl : null));

        CreateMap<Message, MessageDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty))
            .ForMember(d => d.AvatarUrl, opt => opt.MapFrom(s => s.User != null ? s.User.AvatarUrl : null))
            .ForMember(d => d.SentAt, opt => opt.MapFrom(s => s.CreatedAt))
            .ForMember(d => d.Attachments, opt => opt.MapFrom(s => s.Attachments))
            .ForMember(d => d.Reactions, opt => opt.MapFrom(s => s.Reactions))
            .ForMember(d => d.LinkedTaskId,
                opt => opt.MapFrom(s => s.MessageTasks.Select(mt => (Guid?)mt.TaskID).FirstOrDefault()));

        CreateMap<MessageAttachment, MessageAttachmentDto>();

        CreateMap<MessageReaction, MessageReactionDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : string.Empty));
    }
}