using AutoMapper;
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Domain.Entities;

namespace MessageCenter.Application.Mappings;

public class MessageTemplateMappingProfile : Profile
{
    public MessageTemplateMappingProfile()
    {
        CreateMap<MessageTemplate, MessageTemplateDto>();
        CreateMap<CreateMessageTemplateDto, MessageTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
