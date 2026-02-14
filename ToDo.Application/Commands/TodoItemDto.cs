using ToDo.Domain.Enums;

namespace ToDo.Application.Commands;

public record TodoItemDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    Priority Priority,
    DateTime CreatedAt,
    DateTime? CompletedAt
);