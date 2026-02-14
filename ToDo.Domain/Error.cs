namespace ToDo.Domain;

public record Error(string Code, string Message)
{
    public static Error None => new(string.Empty, string.Empty);
    public static Error Validation => new("Validation.Failure", "A validation error occurred.");
    public static Error NotFound => new("Entity.NotFound", "Entity was not found.");
    public static Error Conflict => new("Entity.Conflict", "Entity already exists.");
    public static Error Unexpected => new("Unexpected.Error", "An unexpected error occurred.");
}