using MediatR;
using ToDo.Application.Interfaces;
using ToDo.Domain;

namespace ToDo.Application.Commands;

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTodoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var existingTodo = await _unitOfWork.TodoRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (existingTodo is null)
        {
            return Result.Failure(new Error("Todo.NotFound", $"Todo with ID {request.Id} was not found"));
        }

        await _unitOfWork.TodoRepository.DeleteAsync(existingTodo, cancellationToken);
        
        var saveResult = await _unitOfWork.SaveChangesWithResultAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult;
        }

        return Result.Success();
    }
}