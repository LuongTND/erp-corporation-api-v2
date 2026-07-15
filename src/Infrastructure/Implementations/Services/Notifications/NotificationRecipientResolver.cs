namespace Infrastructure;

public class NotificationRecipientResolver : INotificationRecipientResolver
{
    private const string SuperAdminRoleName = "ROLE_SUPER_ADMIN";

    private readonly AppDbContext _context;

    public NotificationRecipientResolver(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Guid>> ResolveAsync(
        NotificationRecipientRulesDto rules,
        NotificationPublishContext context,
        CancellationToken cancellationToken = default)
    {
        var recipientIds = new HashSet<Guid>();

        if (rules.IncludeSubjectUser && context.SubjectUserId is Guid subjectId)
            recipientIds.Add(subjectId);

        if (rules.IncludeActor && context.ActorUserId is Guid actorId)
            recipientIds.Add(actorId);

        if (rules.IncludeSuperAdmins)
        {
            var superAdminIds = await GetActiveUserIdsByRoleNameAsync(SuperAdminRoleName, cancellationToken);
            foreach (var id in superAdminIds)
                recipientIds.Add(id);
        }

        if (rules.IncludeDepartmentManager && context.SubjectUserId is Guid subjectForManager)
        {
            var managerId = await GetDepartmentManagerIdForUserAsync(subjectForManager, cancellationToken);
            if (managerId.HasValue)
                recipientIds.Add(managerId.Value);
        }

        if (rules.RoleIds.Count > 0)
        {
            var roleUserIds = await GetActiveUserIdsByRoleIdsAsync(rules.RoleIds, cancellationToken);
            foreach (var id in roleUserIds)
                recipientIds.Add(id);
        }

        foreach (var userId in rules.UserIds)
            recipientIds.Add(userId);

        return recipientIds.ToList();
    }

    private async Task<IReadOnlyList<Guid>> GetActiveUserIdsByRoleNameAsync(
        string roleName,
        CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.IsActive && ur.Role.IsActive && ur.Role.RoleName == roleName)
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<Guid>> GetActiveUserIdsByRoleIdsAsync(
        IReadOnlyList<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.IsActive && roleIds.Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private async Task<Guid?> GetDepartmentManagerIdForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var departmentId = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.DepartmentId)
            .FirstOrDefaultAsync(cancellationToken);

        if (departmentId == Guid.Empty)
            return null;

        return await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == departmentId && d.IsActive)
            .Select(d => d.ManagerId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}