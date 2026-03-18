using Core.Domain.Results;

namespace Core.Domain.Extensions;

/// <summary>
/// Extension methods for Result pattern matching (pure domain logic)
/// </summary>
public static class ResultExtensions
{
	/// <summary>
	/// Pattern matching for Result
	/// </summary>
	public static TOut Match<TOut>(this Result result, Func<TOut> onSuccess, Func<Result, TOut> onFailure)
	{
		return result.IsSuccess ? onSuccess() : onFailure(result);
	}

	/// <summary>
	/// Pattern matching for Result{T}
	/// </summary>
	public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess, Func<Result<TIn>, TOut> onFailure)
	{
		return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
	}
}
