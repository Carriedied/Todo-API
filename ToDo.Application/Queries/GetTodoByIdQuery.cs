using MediatR;
using ToDo.Domain;

namespace ToDo.Application.Commands.Queries;

public record GetTodoByIdQuery(int Id) : IRequest<ResultWithValue<TodoItemDto>>;