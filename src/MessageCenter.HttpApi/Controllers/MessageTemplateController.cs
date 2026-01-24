using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace MessageCenter.HttpApi.Controllers;

/// <summary>
/// 消息模板控制器
/// 提供消息模板管理的完整API接口，支持模板的创建、查询、更新、删除以及启用/禁用等功能
/// </summary>
[ApiController]
[Route("api/message-templates")]
[Produces("application/json")]
public class MessageTemplateController(IMessageTemplateAppService templateAppService) : AbpControllerBase
{
    private readonly IMessageTemplateAppService _templateAppService = templateAppService;

    /// <summary>
    /// 创建消息模板
    /// </summary>
    /// <param name="input">创建模板的请求参数</param>
    /// <returns>创建的模板对象</returns>
    /// <response code="200">成功创建模板</response>
    /// <response code="400">请求参数无效或模板代码已存在</response>
    /// <response code="401">未授权</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageTemplateDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<MessageTemplateDto> CreateAsync([FromBody] CreateMessageTemplateDto input)
    {
        return await _templateAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新消息模板
    /// </summary>
    /// <param name="id">模板ID</param>
    /// <param name="input">更新模板的请求参数</param>
    /// <returns>更新后的模板对象</returns>
    /// <response code="200">成功更新模板</response>
    /// <response code="400">请求参数无效或模板代码已被其他模板使用</response>
    /// <response code="404">模板不存在</response>
    /// <response code="401">未授权</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MessageTemplateDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<MessageTemplateDto> UpdateAsync(Guid id, [FromBody] CreateMessageTemplateDto input)
    {
        return await _templateAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 根据ID获取消息模板
    /// </summary>
    /// <param name="id">模板ID</param>
    /// <returns>模板对象</returns>
    /// <response code="200">成功获取模板</response>
    /// <response code="404">模板不存在</response>
    /// <response code="401">未授权</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MessageTemplateDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<MessageTemplateDto> GetAsync(Guid id)
    {
        return await _templateAppService.GetAsync(id);
    }

    /// <summary>
    /// 根据代码获取消息模板
    /// </summary>
    /// <param name="code">模板代码</param>
    /// <returns>模板对象</returns>
    /// <response code="200">成功获取模板</response>
    /// <response code="404">模板不存在</response>
    /// <response code="401">未授权</response>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(MessageTemplateDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<MessageTemplateDto> GetByCodeAsync(string code)
    {
        return await _templateAppService.GetByCodeAsync(code);
    }

    /// <summary>
    /// 获取消息模板列表
    /// </summary>
    /// <returns>模板列表</returns>
    /// <response code="200">成功获取模板列表</response>
    /// <response code="401">未授权</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<MessageTemplateDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<List<MessageTemplateDto>> GetListAsync()
    {
        return await _templateAppService.GetListAsync();
    }

    /// <summary>
    /// 删除消息模板
    /// </summary>
    /// <param name="id">模板ID</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功删除模板</response>
    /// <response code="404">模板不存在</response>
    /// <response code="401">未授权</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task DeleteAsync(Guid id)
    {
        await _templateAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 启用/禁用消息模板
    /// </summary>
    /// <param name="id">模板ID</param>
    /// <param name="isEnabled">是否启用，true为启用，false为禁用</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功设置模板状态</response>
    /// <response code="404">模板不存在</response>
    /// <response code="401">未授权</response>
    [HttpPut("{id}/enabled")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task SetEnabledAsync(Guid id, [FromBody] bool isEnabled)
    {
        await _templateAppService.SetEnabledAsync(id, isEnabled);
    }
}
