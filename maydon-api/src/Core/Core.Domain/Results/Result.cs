namespace Core.Domain.Results;

public class Result : IOperationResult
{
	public Result(bool isSuccess, Error error)
	{
		if (isSuccess && error != Error.None)
			isSuccess = false;

		IsSuccess = isSuccess;
		Error = error;
	}

	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;
	public Error Error { get; }
	public static Result Success() => new(true, Error.None);
	public static Result<TValue> Success<TValue>(TValue? value) => new(value, true, Error.None);
	public static Result Failure(Error error) => new(false, error);
	public static Result Failure(string code, string description) => new(false, new Error(code, description, ErrorType.Failure));
	public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
	public static Result<TValue> Failure<TValue>(string code, string description) => new(default, false, new Error(code, description, ErrorType.Failure));
	public static Result None => new Result(false, Error.None);
}

