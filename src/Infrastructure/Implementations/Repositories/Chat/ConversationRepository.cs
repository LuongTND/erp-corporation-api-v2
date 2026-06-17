using Application.Interfaces.Repositories.Chat;
using Domain.Entities.Chat;
using Domain.Enums.Chat;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementations.Repositories.Chat;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Conversation?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<List<Conversation>> GetUserConversationsAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .AsSplitQuery()
            .Where(c => c.IsActive && c.Members.Any(m => m.UserID == userId && m.IsActive))
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task<Conversation?> GetDirectConversationAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
    {
        return await DbSet
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.ConversationType == ConversationType.Direct &&
                c.Members.Any(m => m.UserID == userId1 && m.IsActive) &&
                c.Members.Any(m => m.UserID == userId2 && m.IsActive), ct);
    }
}
