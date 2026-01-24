using MessageCenter.Application.Contracts.DTOs;
using Volo.Abp.Application.Services;

namespace MessageCenter.Application.Contracts.Services;

/// <summary>
/// 消息模板应用服务接口
/// </summary>
public interface IMessageTemplateAppService : IApplicationService
{
    /// <summary>
    /// 创建消息模板
    /// </summary>
    Task<MessageTemplateDto> CreateAsync(CreateMessageTemplateDto input);

    /// <summary>
    /// 更新消息模板
    /// </summary>
    Task<MessageTemplateDto> UpdateAsync(Guid id, CreateMessageTemplateDto input);

    /// <summary>
    /// 根据ID获取消息模板
    /// </summary>
    Task<MessageTemplateDto> GetAsync(Guid id);

    /// <summary>
    /// 根据代码获取消息模板
    /// </summary>
    Task<MessageTemplateDto> GetByCodeAsync(string code);

    /// <summary>
    /// 获取消息模板列表
    /// </summary>
    Task<List<MessageTemplateDto>> GetListAsync();

    /// <summary>
    /// 删除消息模板
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// 启用/禁用消息模板
    /// </summary>
    Task SetEnabledAsync(Guid id, bool isEnabled);
}
