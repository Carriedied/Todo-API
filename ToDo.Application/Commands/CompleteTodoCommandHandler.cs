using MediatR;
using ToDo.Application.Interfaces;
using ToDo.Domain;

namespace ToDo.Application.Commands;

public class CompleteTodoCommandHandler : IRequestHandler<CompleteTodoCommand, ResultWithValue<TodoItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultWithValue<TodoItemDto>> Handle(CompleteTodoCommand request, CancellationToken cancellationToken)
    {
        var existingTodo = await _unitOfWork.TodoRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (existingTodo is null)
        {
            return Result.Failure<TodoItemDto>(
                new Error("Todo.NotFound", $"Todo with ID {request.Id} was not found"));
        }

        var completeResult = existingTodo.Complete(DateTime.UtcNow);
        if (completeResult.IsFailure)
        {
            return Result.Failure<TodoItemDto>(completeResult.Error);
        }

        await _unitOfWork.TodoRepository.UpdateAsync(existingTodo, cancellationToken);
        
        var saveResult = await _unitOfWork.SaveChangesWithResultAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return Result.Failure<TodoItemDto>(saveResult.Error);
        }

        return Result.Success(MapToDto(existingTodo));
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