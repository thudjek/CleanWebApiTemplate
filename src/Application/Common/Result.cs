namespace Application.Common;
public class Result
{
    public Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; protected set; }
    public string Error { get; protected set; }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Failure()
    {
        return new Result(false);
    }

    public static Result Failure(string error)
    {
        return new Result(error);
    }

    public ErrorModel ToErrorModel()
    {
        return new ErrorModel(Error);
    }
}

public class Result<T> : Result where T : class
{
    public Result(T value) : base(true)
    {
        Value = value;
    }

    public Result(string error) : base(error)
    {
    }

    public T Value { get; private set; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public new static Result<T> Failure(string error)
    {
        return new Result<T>(error);
    }

    public new static Result<T> Failure()
    {
        return new Result<T>(string.Empty);
    }
}