using Microsoft.EntityFrameworkCore;
using ToDo.Application.Interfaces;
using ToDo.Domain;
using ToDo.Domain.Entities;
using ToDo.Domain.Events;
using ToDo.Infrastructure.Persistence;

namespace ToDo.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ITodoRepository TodoRepository => new TodoRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> SaveChangesWithResultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateException ex)
        {
            return Result.Failure(new Error("Persistence.Conflict", ex.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Persistence.Unexpected", ex.Message));
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}