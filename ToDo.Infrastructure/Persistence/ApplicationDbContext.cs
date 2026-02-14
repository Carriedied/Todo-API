using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Events;

namespace ToDo.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public async Task PublishEventAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        await _publisher.Publish(@event, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = new List<object>();

        foreach (var entry in ChangeTracker.Entries<TodoItem>())
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var events = entry.Entity.GetAndClearDomainEvents();
                domainEvents.AddRange(events);
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<TodoItem>().HasData(
            new
            {
                Id = 1,
                Title = "Sample todo item",
                Description = "This is a sample todo item for demonstration",
                IsCompleted = false,
                Priority = Domain.Enums.Priority.Medium,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CompletedAt = (DateTime?)null
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}