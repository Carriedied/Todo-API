using FluentValidation;
using ToDo.Application.Commands.Queries;

namespace ToDo.Application.Validators;

public class GetTodoByIdQueryValidator : AbstractValidator<GetTodoByIdQuery>
{
    public GetTodoByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Todo ID must not be empty");
    }
}