namespace Core.Domain.Results;

public interface IOperationResult
{
	bool IsSuccess { get; }
	bool IsFailure { get; }
	Error Error { get; }
}
