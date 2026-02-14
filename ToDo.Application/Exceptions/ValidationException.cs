using FluentValidation.Results;

namespace ToDo.Application.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(IEnumerable<ValidationFailure> failures)
    {
        Failures = failures.ToDictionary(x => x.PropertyName, x => x.ErrorMessage);
    }

    public IDictionary<string, string> Failures { get; }
}