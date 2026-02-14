using MediatR;
using ToDo.Domain;

namespace ToDo.Application.Commands;

public record UpdateTodoCommand(
    int Id,
    string Title,
    string? Description,
    Domain.Enums.Priority Priority
) : IRequest<ResultWithValue<TodoItemDto>>;