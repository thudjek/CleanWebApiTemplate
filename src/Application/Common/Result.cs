namespace Application.Common;
public class Result
{
    protected Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    protected Result(string errorMessage)
    {
        IsSuccess = false;
        Error = new Error(errorMessage);
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public bool IsSuccess { get; protected set; }
    public Error Error { get; protected set; }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Fail(string errorMessage)
    {
        return new Result(errorMessage);
    }

    public static Result Fail(Error error)
    {
        return new Result(error);
    }
}

public class Result<T> : Result where T : class
{
    private Result(T value) : base(true)
    {
        Value = value;
    }

    private Result(string errorMessage) : base(errorMessage)
    {
    }

    private Result(Error error) : base(error)
    {
    }

    public T Value { get; private set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public new static Result<T> Fail(string errorMessage)
    {
        return new Result<T>(errorMessage);
    }

    public new static Result<T> Fail(Error error)
    {
        return new Result<T>(error);
    }
}