using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Repositories;
using MessageCenter.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MessageCenter.EntityFrameworkCore.Repositories;

public class MessageChannelConfigRepository(IDbContextProvider<MessageCenterDbContext> dbContextProvider) : EfCoreRepository<MessageCenterDbContext, MessageChannelConfig, Guid>(dbContextProvider), IMessageChannelConfigRepository
{
    public async Task<MessageChannelConfig?> GetDefaultConfigAsync(
        MessageChannel channel,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .FirstOrDefaultAsync(x => 
                x.Channel == channel && 
                x.IsDefault && 
                x.IsEnabled, 
                cancellationToken);
    }

    public async Task<List<MessageChannelConfig>> GetEnabledConfigsAsync(
        MessageChannel channel,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .Where(x => x.Channel == channel && x.IsEnabled)
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
