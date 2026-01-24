using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Repositories;
using Volo.Abp.Application.Services;

namespace MessageCenter.Application.Services;

/// <summary>
/// 消息模板应用服务实现
/// </summary>
public class MessageTemplateAppService(IMessageTemplateRepository templateRepository) : ApplicationService, IMessageTemplateAppService
{
    private readonly IMessageTemplateRepository _templateRepository = templateRepository;

    public virtual async Task<MessageTemplateDto> CreateAsync(CreateMessageTemplateDto input)
    {
        // 检查模板代码是否已存在
        var existing = await _templateRepository.GetByCodeAsync(input.Code);
        if (existing != null)
        {
            throw new InvalidOperationException($"模板代码 {input.Code} 已存在");
        }

        var template = ObjectMapper.Map<CreateMessageTemplateDto, MessageTemplate>(input);
        await _templateRepository.InsertAsync(template);

        return ObjectMapper.Map<MessageTemplate, MessageTemplateDto>(template);
    }

    public virtual async Task<MessageTemplateDto> UpdateAsync(Guid id, CreateMessageTemplateDto input)
    {
        var template = await _templateRepository.GetAsync(id);

        // 检查模板代码是否被其他模板使用
        if (template.Code != input.Code)
        {
            var existing = await _templateRepository.GetByCodeAsync(input.Code);
            if (existing != null && existing.Id != id)
            {
                throw new InvalidOperationException($"模板代码 {input.Code} 已被其他模板使用");
            }
        }

        template.Name = input.Name;
        template.Code = input.Code;
        template.TemplateType = input.TemplateType;
        template.MessageType = input.MessageType;
        template.Channel = input.Channel;
        template.Title = input.Title;
        template.Content = input.Content;
        template.Description = input.Description;
        template.IsEnabled = input.IsEnabled;
        template.Variables = input.Variables;
        template.Extension = input.Extension;

        await _templateRepository.UpdateAsync(template);

        return ObjectMapper.Map<MessageTemplate, MessageTemplateDto>(template);
    }

    public virtual async Task<MessageTemplateDto> GetAsync(Guid id)
    {
        var template = await _templateRepository.GetAsync(id);
        return ObjectMapper.Map<MessageTemplate, MessageTemplateDto>(template);
    }

    public virtual async Task<MessageTemplateDto> GetByCodeAsync(string code)
    {
        var template = await _templateRepository.GetByCodeAsync(code);
        if (template == null)
        {
            throw new InvalidOperationException($"模板代码 {code} 不存在");
        }

        return ObjectMapper.Map<MessageTemplate, MessageTemplateDto>(template);
    }

    public virtual async Task<List<MessageTemplateDto>> GetListAsync()
    {
        var templates = await _templateRepository.GetListAsync();
        return ObjectMapper.Map<List<MessageTemplate>, List<MessageTemplateDto>>(templates);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await _templateRepository.DeleteAsync(id);
    }

    public virtual async Task SetEnabledAsync(Guid id, bool isEnabled)
    {
        var template = await _templateRepository.GetAsync(id);
        template.IsEnabled = isEnabled;
        await _templateRepository.UpdateAsync(template);
    }
}
