using MediatR;
using ToDo.Domain;

namespace ToDo.Application.Commands.Queries;

public record GetTodosQuery(
    bool? IsCompleted,
    Domain.Enums.Priority? Priority,
    int Page,
    int PageSize
) : IRequest<ResultWithValue<PagedResult<TodoItemDto>>>;