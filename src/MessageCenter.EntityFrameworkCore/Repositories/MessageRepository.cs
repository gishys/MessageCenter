using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Repositories;
using MessageCenter.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MessageCenter.EntityFrameworkCore.Repositories;

public class MessageRepository(IDbContextProvider<MessageCenterDbContext> dbContextProvider) : EfCoreRepository<MessageCenterDbContext, Message, Guid>(dbContextProvider), IMessageRepository
{
    public async Task<List<Message>> GetByReceiverIdAsync(
        string receiverId,
        MessageStatus? status = null,
        MessageType? messageType = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(x => x.ReceiverId == receiverId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (messageType.HasValue)
        {
            query = query.Where(x => x.MessageType == messageType.Value);
        }

        return await query
            .OrderByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Message>> GetBySenderIdAsync(
        Guid senderId,
        MessageStatus? status = null,
        int skipCount = 0,
        int maxResultCount = 10,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(x => x.SenderId == senderId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Message>> GetPendingMessagesAsync(
        MessageChannel? channel = null,
        int maxResultCount = 100,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(x => x.Status == MessageStatus.Pending);

        if (channel.HasValue)
        {
            query = query.Where(x => x.Channel == channel.Value);
        }

        var now = DateTime.UtcNow;
        query = query.Where(x => !x.ScheduledSendTime.HasValue || x.ScheduledSendTime.Value <= now);

        return await query
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.CreationTime)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Message>> GetRetryMessagesAsync(
        int maxResultCount = 100,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(x => 
            x.Status == MessageStatus.Failed && 
            x.RetryCount < x.MaxRetryCount);

        return await query
            .OrderBy(x => x.RetryCount)
            .ThenBy(x => x.CreationTime)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message?> GetByBusinessAsync(
        string businessType,
        string businessId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .FirstOrDefaultAsync(x => 
                x.BusinessType == businessType && 
                x.BusinessId == businessId, 
                cancellationToken);
    }

    public async Task<long> CountUnreadAsync(
        string receiverId,
        MessageType? messageType = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(x => 
            x.ReceiverId == receiverId && 
            x.Status != MessageStatus.Read);

        if (messageType.HasValue)
        {
            query = query.Where(x => x.MessageType == messageType.Value);
        }

        return await query.LongCountAsync(cancellationToken);
    }

    public async Task<Dictionary<MessageStatus, long>> GetStatusStatisticsAsync(
        string? receiverId = null,
        MessageType? messageType = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(receiverId))
        {
            query = query.Where(x => x.ReceiverId == receiverId);
        }

        if (messageType.HasValue)
        {
            query = query.Where(x => x.MessageType == messageType.Value);
        }

        if (startTime.HasValue)
        {
            query = query.Where(x => x.CreationTime >= startTime.Value);
        }

        if (endTime.HasValue)
        {
            query = query.Where(x => x.CreationTime <= endTime.Value);
        }

        var statistics = await query
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.LongCount() })
            .ToListAsync(cancellationToken);

        return statistics.ToDictionary(x => x.Status, x => x.Count);
    }
}
