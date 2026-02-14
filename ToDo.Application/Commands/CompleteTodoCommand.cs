using MediatR;
using ToDo.Domain;

namespace ToDo.Application.Commands;

public record CompleteTodoCommand(int Id) : IRequest<ResultWithValue<TodoItemDto>>;