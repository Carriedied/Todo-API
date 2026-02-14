using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces;

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoItem>> GetTodosAsync(
        bool? isCompleted, 
        Domain.Enums.Priority? priority,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<TodoItem?> GetByIdAsync(
        int id, 
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TodoItem todoItem, 
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        TodoItem todoItem, 
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        TodoItem todoItem, 
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        bool? isCompleted, 
        Domain.Enums.Priority? priority,
        CancellationToken cancellationToken = default);
}