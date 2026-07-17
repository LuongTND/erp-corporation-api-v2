namespace Infrastructure;

[RegisterService(typeof(IMessageRepository))]
public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Message>> GetPagedMessagesAsync(Guid conversationId, int page, int pageSize,
        CancellationToken ct = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(m => m.User)
            .Include(m => m.Attachments)
            .Include(m => m.Reactions)
            .ThenInclude(r => r.User)
            .Include(m => m.MessageTasks)
            .Where(m => m.ConversationID == conversationId)
            .OrderByDescending(m => m.CreatedAt) // Mới nhất lên trước để phân trang
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Reverse() // Lật ngược lại để hiển thị từ cũ đến mới trên UI
            .ToListAsync(ct);
    }
}