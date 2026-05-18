using Domain.Base;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IMediator _mediator;

    public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
        }

        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = domainEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEvents.ForEach(e => e.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in events)
            await _mediator.Publish(domainEvent, cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
