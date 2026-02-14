using MediatR;
using ToDo.Domain;

namespace ToDo.Application.Commands;

public record DeleteTodoCommand(int Id) : IRequest<Result>;