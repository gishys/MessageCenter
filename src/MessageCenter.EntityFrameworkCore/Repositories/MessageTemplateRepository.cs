using MessageCenter.Domain.Entities;
using MessageCenter.Domain.Repositories;
using MessageCenter.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MessageCenter.EntityFrameworkCore.Repositories;

public class MessageTemplateRepository(IDbContextProvider<MessageCenterDbContext> dbContextProvider) : EfCoreRepository<MessageCenterDbContext, MessageTemplate, Guid>(dbContextProvider), IMessageTemplateRepository
{
    public async Task<MessageTemplate?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        return await query
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<List<MessageTemplate>> GetByTypeAndChannelAsync(
        MessageType messageType,
        MessageChannel channel,
        bool onlyEnabled = true,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = query.Where(x => 
            x.MessageType == messageType && 
            x.Channel == channel);

        if (onlyEnabled)
        {
            query = query.Where(x => x.IsEnabled);
        }

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
