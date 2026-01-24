using AutoMapper;
using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Domain.Entities;
using System.Text.Json;

namespace MessageCenter.Application.Mappings;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.Status == Domain.Shared.Enums.MessageStatus.Read));

        CreateMap<CreateMessageDto, Message>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Shared.Enums.MessageStatus.Pending))
            .ForMember(dest => dest.AttachmentIds, opt => opt.MapFrom((src, dest, destMember, context) => 
                src.AttachmentIds != null && src.AttachmentIds.Count > 0 
                    ? JsonSerializer.Serialize(src.AttachmentIds) 
                    : null))
            .ForMember(dest => dest.Receipts, opt => opt.Ignore());
    }
}
