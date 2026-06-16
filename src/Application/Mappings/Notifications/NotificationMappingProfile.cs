using Application.Common.Notifications;
using Application.DTOs.Notifications;
using AutoMapper;

namespace Application.Mappings.Notifications;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<NotificationEventType, NotificationEventTypeDto>();
        CreateMap<NotificationTemplate, NotificationTemplateDto>();
        CreateMap<UserNotification, UserNotificationDto>();

        CreateMap<NotificationTriggerBinding, NotificationTriggerBindingDto>()
            .ForMember(d => d.EventCode, o => o.MapFrom(s => s.EventType != null ? s.EventType.EventCode : null))
            .ForMember(d => d.EventTypeName, o => o.MapFrom(s => s.EventType != null ? s.EventType.Name : null))
            .ForMember(d => d.RecipientRules, o => o.MapFrom(s => NotificationRecipientRulesJson.Deserialize(s.RecipientRulesJson)));
    }
}
