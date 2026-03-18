using System.Diagnostics.CodeAnalysis;

namespace Core.Domain.Results;

public class Result<TValue> : Result
{
	private readonly TValue? _value;

	public Result(TValue? value, bool isSuccess, Error error)
		: base(isSuccess, error) => _value = value;

	[NotNull]
	public TValue Value => IsSuccess ? _value! : default;

	public static implicit operator Result<TValue>(TValue? value) =>
		value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

	public static Result<TValue> ValidationFailure(Error error) => new(default, false, error);

	public static new Result<TValue> None => new Result<TValue>(default, false, Error.None);
}
