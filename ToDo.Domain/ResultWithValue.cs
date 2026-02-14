namespace ToDo.Domain;

public class ResultWithValue<TValue> : Result
{
    private readonly TValue? _value;

    protected internal ResultWithValue(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess ? _value! : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator ResultWithValue<TValue>(TValue value) => Success(value);
} 