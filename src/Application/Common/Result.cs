using System.Net;

namespace Application.Common;
public class Result
{
    protected Result(bool isSuccess, HttpStatusCode statusCode)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
    }

    protected Result(string error, HttpStatusCode statusCode)
    {
        IsSuccess = false;
        Error = error;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; protected set; }
    public string Error { get; protected set; } = "Something went wrong, please try again.";
    public HttpStatusCode StatusCode { get; protected set; }

    public static Result Success(HttpStatusCode? statusCode = null)
    {
        return new Result(true, statusCode ?? HttpStatusCode.OK);
    }

    public static Result Failure(HttpStatusCode? statusCode = null)
    {
        return new Result(false, statusCode ?? HttpStatusCode.BadRequest);
    }

    public static Result Failure(string error, HttpStatusCode? statusCode = null)
    {
        return new Result(error, statusCode ?? HttpStatusCode.BadRequest);
    }

    public ErrorModel ToErrorModel()
    {
        return new ErrorModel(Error);
    }
}

public class Result<T> : Result where T : class
{
    private Result(T value, HttpStatusCode statusCode) : base(true, statusCode)
    {
        Value = value;
    }

    private Result(HttpStatusCode statusCode) : base(false, statusCode)
    { 
    }

    private Result(string error, HttpStatusCode statusCode) : base(error, statusCode)
    {
    }

    public T Value { get; private set; }

    public static Result<T> Success(T value, HttpStatusCode? statusCode = null)
    {
        return new Result<T>(value, statusCode ?? HttpStatusCode.OK);
    }

    public new static Result<T> Failure(HttpStatusCode? statusCode = null)
    {
        return new Result<T>(statusCode ?? HttpStatusCode.BadRequest);
    }

    public new static Result<T> Failure(string error, HttpStatusCode? statusCode = null)
    {
        return new Result<T>(error, statusCode ?? HttpStatusCode.BadRequest);
    }
}