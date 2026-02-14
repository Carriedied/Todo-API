using ToDo.Domain;

namespace ToDo.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITodoRepository TodoRepository { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Result> SaveChangesWithResultAsync(CancellationToken cancellationToken = default);
}