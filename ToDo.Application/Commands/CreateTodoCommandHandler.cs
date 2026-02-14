using MediatR;
using ToDo.Application.Interfaces;
using ToDo.Domain;
using ToDo.Domain.Entities;

namespace ToDo.Application.Commands;

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, ResultWithValue<TodoItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultWithValue<TodoItemDto>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todoItem = TodoItem.Create(request.Title, request.Description, request.Priority, DateTime.UtcNow);
        
        if (todoItem.IsFailure)
        {
            return Result.Failure<TodoItemDto>(todoItem.Error);
        }

        await _unitOfWork.TodoRepository.AddAsync(todoItem.Value, cancellationToken);
        
        var saveResult = await _unitOfWork.SaveChangesWithResultAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return Result.Failure<TodoItemDto>(saveResult.Error);
        }

        return Result.Success(MapToDto(todoItem.Value));
    }

    private static TodoItemDto MapToDto(TodoItem todoItem) =>
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