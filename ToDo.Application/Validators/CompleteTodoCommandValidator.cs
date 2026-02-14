using FluentValidation;
using ToDo.Application.Commands;

namespace ToDo.Application.Validators;

public class CompleteTodoCommandValidator : AbstractValidator<CompleteTodoCommand>
{
    public CompleteTodoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Todo ID must not be empty");
    }
}