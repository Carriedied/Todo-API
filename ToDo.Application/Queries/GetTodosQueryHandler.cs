using MediatR;
using ToDo.Application.Interfaces;
using ToDo.Domain;

namespace ToDo.Application.Commands.Queries;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, ResultWithValue<PagedResult<TodoItemDto>>>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodosQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<ResultWithValue<PagedResult<TodoItemDto>>> Handle(GetTodosQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await _todoRepository.CountAsync(
            request.IsCompleted,
            request.Priority,
            cancellationToken);

        var todos = await _todoRepository.GetTodosAsync(
            request.IsCompleted,
            request.Priority,
            request.Page,
            request.PageSize,
            cancellationToken);

        var todoDtos = todos.Select(MapToDto).ToList();

        var pagedResult = new PagedResult<TodoItemDto>(
            todoDtos,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result.Success(pagedResult);
    }

    private static TodoItemDto MapToDto(Domain.Entities.TodoItem todoItem) =>
        new(
            todoItem.Id,
            todoItem.Title,
            todoItem.Description,
            todoItem.IsCompleted,
            todoItem.Priority,
            todoItem.CreatedAt,
            todoItem.CompletedAt
        );
}