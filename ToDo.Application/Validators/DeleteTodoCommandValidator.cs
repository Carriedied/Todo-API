using FluentValidation;
using ToDo.Application.Commands;

namespace ToDo.Application.Validators;

public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Todo ID must not be empty");
    }
}