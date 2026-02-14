using MediatR;
using ToDo.Application.Interfaces;
using ToDo.Domain;

namespace ToDo.Application.Commands.Queries;

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, ResultWithValue<TodoItemDto>>
{
    private readonly ITodoRepository _todoRepository;

    public GetTodoByIdQueryHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task<ResultWithValue<TodoItemDto>> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
    {
        var todo = await _todoRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (todo is null)
        {
            return Result.Failure<TodoItemDto>(
                new Error("Todo.NotFound", $"Todo with ID {request.Id} was not found"));
        }

        return Result.Success(MapToDto(todo));
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