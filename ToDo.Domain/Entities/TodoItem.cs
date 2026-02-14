using ToDo.Domain.Enums;
using ToDo.Domain.Events;

namespace ToDo.Domain.Entities;

public class TodoItem
{
    public int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsCompleted { get; private set; }
    public Priority Priority { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    
    private readonly List<object> _domainEvents = new();
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    private TodoItem() { }

    private TodoItem(string title, string? description, Priority priority, DateTime createdAt)
    {
        Title = title;
        Description = description;
        Priority = priority;
        IsCompleted = false;
        CreatedAt = createdAt;
        CompletedAt = null;
    }

    public static ResultWithValue<TodoItem> Create(string title, string? description, Priority priority, DateTime createdAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<TodoItem>(new Error("Todo.Title.Required", "Title is required"));
        }

        if (title.Length > 200)
        {
            return Result.Failure<TodoItem>(new Error("Todo.Title.TooLong", "Title cannot exceed 200 characters"));
        }

        if (description is not null && description.Length > 1000)
        {
            return Result.Failure<TodoItem>(new Error("Todo.Description.TooLong", "Description cannot exceed 1000 characters"));
        }

        var todoItem = new TodoItem(title, description, priority, createdAt);

        return Result.Success(todoItem);
    }

    public Result Complete(DateTime completedAt)
    {
        if (IsCompleted)
        {
            return Result.Failure(new Error("Todo.AlreadyCompleted", "Todo item is already completed"));
        }

        if (completedAt == default)
        {
            return Result.Failure(new Error("Todo.InvalidDateTime", "Completion date cannot be default"));
        }

        IsCompleted = true;
        CompletedAt = completedAt;

        return Result.Success();
    }

    public Result UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(new Error("Todo.Title.Required", "Title is required"));
        }

        if (title.Length > 200)
        {
            return Result.Failure(new Error("Todo.Title.TooLong", "Title cannot exceed 200 characters"));
        }

        Title = title;
        return Result.Success();
    }

    public Result UpdateDescription(string? description)
    {
        if (description is not null && description.Length > 1000)
        {
            return Result.Failure(new Error("Todo.Description.TooLong", "Description cannot exceed 1000 characters"));
        }

        Description = description;
        return Result.Success();
    }

    public Result UpdatePriority(Priority priority)
    {
        Priority = priority;
        return Result.Success();
    }

    public Result Update(string title, string? description, Priority priority)
    {
        var titleResult = UpdateTitle(title);
        if (titleResult.IsFailure)
        {
            return titleResult;
        }

        var descriptionResult = UpdateDescription(description);
        if (descriptionResult.IsFailure)
        {
            return descriptionResult;
        }

        var priorityResult = UpdatePriority(priority);
        if (priorityResult.IsFailure)
        {
            return priorityResult;
        }

        return Result.Success();
    }

    public void PublishCreatedEvent()
    {
        if (Id != 0)
        {
            AddDomainEvent(new TodoCreatedDomainEvent(Id, Title));
        }
    }

    protected void AddDomainEvent(object @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public IReadOnlyList<object> GetAndClearDomainEvents()
    {
        var events = _domainEvents.ToList();
        _domainEvents.Clear();
        return events;
    }
}