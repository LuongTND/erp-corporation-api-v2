using Domain.Base;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Outbox;
using Infrastructure.Persistence.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<JobLevel> JobLevels => Set<JobLevel>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<UserDepartment> UserDepartments => Set<UserDepartment>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = domainEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEvents.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in events)
        {
            var (type, payload) = OutboxSerializer.Serialize(domainEvent);
            OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = type,
                Payload = payload,
                OccurredOn = domainEvent.OccurredOn,
                CreatedAt = DateTime.UtcNow
            });
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
