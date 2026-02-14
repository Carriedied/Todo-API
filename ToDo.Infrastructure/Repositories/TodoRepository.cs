using Microsoft.EntityFrameworkCore;
using ToDo.Application.Interfaces;
using ToDo.Domain.Entities;
using ToDo.Infrastructure.Persistence;

namespace ToDo.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly ApplicationDbContext _context;

    public TodoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TodoItem>> GetTodosAsync(
        bool? isCompleted, 
        Domain.Enums.Priority? priority,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TodoItems.AsQueryable();

        if (isCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == isCompleted.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }
        
        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(
        int id, 
        CancellationToken cancellationToken = default)
    {
        return await _context.TodoItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        TodoItem todoItem, 
        CancellationToken cancellationToken = default)
    {
        await _context.TodoItems.AddAsync(todoItem, cancellationToken);
    }

    public async Task UpdateAsync(
        TodoItem todoItem, 
        CancellationToken cancellationToken = default)
    {
        _context.TodoItems.Update(todoItem);
    }

    public async Task DeleteAsync(
        TodoItem todoItem, 
        CancellationToken cancellationToken = default)
    {
        _context.TodoItems.Remove(todoItem);
    }

    public async Task<int> CountAsync(
        bool? isCompleted, 
        Domain.Enums.Priority? priority,
        CancellationToken cancellationToken = default)
    {
        var query = _context.TodoItems.AsQueryable();

        if (isCompleted.HasValue)
        {
            query = query.Where(t => t.IsCompleted == isCompleted.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(t => t.Priority == priority.Value);
        }
        
        return await query.AsNoTracking().CountAsync(cancellationToken);
    }
}