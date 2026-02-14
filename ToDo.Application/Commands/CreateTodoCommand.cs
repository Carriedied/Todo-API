using MediatR;
using ToDo.Domain;

namespace ToDo.Application.Commands;

public record CreateTodoCommand(
    string Title,
    string? Description,
    Domain.Enums.Priority Priority
) : IRequest<ResultWithValue<TodoItemDto>>;