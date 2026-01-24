using MessageCenter.Application.Contracts.DTOs;
using MessageCenter.Application.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MessageCenter.HttpApi.Controllers;

/// <summary>
/// 消息控制器
/// 提供消息中心管理的完整API接口，支持消息的创建、查询、更新、删除以及统计等功能
/// </summary>
[ApiController]
[Route("api/messages")]
[Produces("application/json")]
public class MessageController(IMessageAppService messageAppService) : AbpControllerBase
{
    private readonly IMessageAppService _messageAppService = messageAppService;

    /// <summary>
    /// 创建并发送消息
    /// </summary>
    /// <param name="input">创建消息的请求参数</param>
    /// <returns>创建的消息对象</returns>
    /// <response code="200">成功创建消息</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="401">未授权</response>
    /// <response code="500">服务器内部错误</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<MessageDto> CreateAsync([FromBody] CreateMessageDto input)
    {
        return await _messageAppService.CreateAsync(input);
    }

    /// <summary>
    /// 批量创建并发送消息
    /// </summary>
    /// <param name="inputs">创建消息的请求参数列表（最多1000条）</param>
    /// <returns>创建的消息对象列表</returns>
    /// <response code="200">成功创建消息</response>
    /// <response code="400">请求参数无效或超过批量限制</response>
    /// <response code="401">未授权</response>
    [HttpPost("batch")]
    [ProducesResponseType(typeof(List<MessageDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<List<MessageDto>> CreateBatchAsync([FromBody] List<CreateMessageDto> inputs)
    {
        return await _messageAppService.CreateBatchAsync(inputs);
    }

    /// <summary>
    /// 根据ID获取消息
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>消息对象</returns>
    /// <response code="200">成功获取消息</response>
    /// <response code="404">消息不存在</response>
    /// <response code="401">未授权</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MessageDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task<MessageDto> GetAsync(Guid id)
    {
        return await _messageAppService.GetAsync(id);
    }

    /// <summary>
    /// 查询消息列表
    /// </summary>
    /// <param name="input">查询参数，支持多种筛选条件和分页</param>
    /// <returns>分页的消息列表</returns>
    /// <response code="200">成功获取消息列表</response>
    /// <response code="401">未授权</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<MessageDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<PagedResultDto<MessageDto>> GetListAsync([FromQuery] MessageQueryDto input)
    {
        return await _messageAppService.GetListAsync(input);
    }

    /// <summary>
    /// 获取接收者的消息列表
    /// </summary>
    /// <param name="receiverId">接收者ID</param>
    /// <param name="input">查询参数（可选）</param>
    /// <returns>分页的消息列表</returns>
    /// <response code="200">成功获取消息列表</response>
    /// <response code="401">未授权</response>
    [HttpGet("receiver/{receiverId}")]
    [ProducesResponseType(typeof(PagedResultDto<MessageDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<PagedResultDto<MessageDto>> GetReceiverMessagesAsync(
        string receiverId,
        [FromQuery] MessageQueryDto? input = null)
    {
        return await _messageAppService.GetReceiverMessagesAsync(receiverId, input);
    }

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功标记为已读</response>
    /// <response code="404">消息不存在</response>
    /// <response code="401">未授权</response>
    [HttpPut("{id}/read")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task MarkAsReadAsync(Guid id)
    {
        await _messageAppService.MarkAsReadAsync(id);
    }

    /// <summary>
    /// 批量标记消息为已读
    /// </summary>
    /// <param name="ids">消息ID列表</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功标记为已读</response>
    /// <response code="401">未授权</response>
    [HttpPut("read/batch")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task MarkAsReadBatchAsync([FromBody] List<Guid> ids)
    {
        await _messageAppService.MarkAsReadBatchAsync(ids);
    }

    /// <summary>
    /// 标记所有消息为已读
    /// </summary>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功标记所有消息为已读</response>
    /// <response code="401">未授权</response>
    [HttpPut("read/all/{receiverId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task MarkAllAsReadAsync(string receiverId)
    {
        await _messageAppService.MarkAllAsReadAsync(receiverId);
    }

    /// <summary>
    /// 删除消息
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功删除消息</response>
    /// <response code="404">消息不存在</response>
    /// <response code="401">未授权</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task DeleteAsync(Guid id)
    {
        await _messageAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 批量删除消息
    /// </summary>
    /// <param name="ids">消息ID列表</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功删除消息</response>
    /// <response code="401">未授权</response>
    [HttpDelete("batch")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task DeleteBatchAsync([FromBody] List<Guid> ids)
    {
        await _messageAppService.DeleteBatchAsync(ids);
    }

    /// <summary>
    /// 获取未读消息数量
    /// </summary>
    /// <param name="receiverId">接收者ID</param>
    /// <returns>未读消息数量</returns>
    /// <response code="200">成功获取未读消息数量</response>
    /// <response code="401">未授权</response>
    [HttpGet("unread-count/{receiverId}")]
    [ProducesResponseType(typeof(long), 200)]
    [ProducesResponseType(401)]
    public async Task<long> GetUnreadCountAsync(string receiverId)
    {
        return await _messageAppService.GetUnreadCountAsync(receiverId);
    }

    /// <summary>
    /// 获取消息统计信息
    /// </summary>
    /// <param name="receiverId">接收者ID（可选）</param>
    /// <param name="startTime">开始时间（可选）</param>
    /// <param name="endTime">结束时间（可选）</param>
    /// <returns>消息统计信息，包括按状态、类型、渠道的统计</returns>
    /// <response code="200">成功获取统计信息</response>
    /// <response code="401">未授权</response>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(MessageStatisticsDto), 200)]
    [ProducesResponseType(401)]
    public async Task<MessageStatisticsDto> GetStatisticsAsync(
        [FromQuery] string? receiverId = null,
        [FromQuery] DateTime? startTime = null,
        [FromQuery] DateTime? endTime = null)
    {
        return await _messageAppService.GetStatisticsAsync(receiverId, startTime, endTime);
    }

    /// <summary>
    /// 重试发送失败的消息
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功触发重试</response>
    /// <response code="400">消息不能重试（已达到最大重试次数或状态不允许）</response>
    /// <response code="404">消息不存在</response>
    /// <response code="401">未授权</response>
    [HttpPost("{id}/retry")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task RetryAsync(Guid id)
    {
        await _messageAppService.RetryAsync(id);
    }

    /// <summary>
    /// 取消消息发送
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <returns>无返回值</returns>
    /// <response code="200">成功取消消息</response>
    /// <response code="400">已发送的消息不能取消</response>
    /// <response code="404">消息不存在</response>
    /// <response code="401">未授权</response>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(401)]
    public async Task CancelAsync(Guid id)
    {
        await _messageAppService.CancelAsync(id);
    }

    /// <summary>
    /// 获取SignalR连接信息
    /// 返回SignalR Hub的连接地址和认证信息
    /// </summary>
    /// <returns>SignalR连接信息</returns>
    /// <response code="200">成功获取连接信息</response>
    /// <response code="401">未授权</response>
    [HttpGet("realtime/info")]
    [ProducesResponseType(typeof(RealtimeConnectionInfo), 200)]
    [ProducesResponseType(401)]
    public IActionResult GetRealtimeInfo()
    {
        var request = HttpContext.Request;
        var hubUrl = $"{request.Scheme}://{request.Host}/hubs/messages";
        
        var authHeader = HttpContext.Request.Headers.Authorization.ToString();
        var accessToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? authHeader["Bearer ".Length..]
            : authHeader;

        return Ok(new RealtimeConnectionInfo
        {
            HubUrl = hubUrl,
            AccessToken = accessToken,
            SupportedMethods =
            [
                "ReceiveMessage",
                "NotifyNewMessage",
                "MessageStatusChanged"
            ]
        });
    }
}

/// <summary>
/// SignalR连接信息
/// </summary>
public class RealtimeConnectionInfo
{
    /// <summary>
    /// Hub连接地址
    /// </summary>
    public string HubUrl { get; set; } = string.Empty;

    /// <summary>
    /// 访问令牌（用于连接认证）
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// 支持的方法列表
    /// </summary>
    public List<string> SupportedMethods { get; set; } = [];
}
