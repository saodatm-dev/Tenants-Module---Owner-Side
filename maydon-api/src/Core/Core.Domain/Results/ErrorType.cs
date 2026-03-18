namespace Core.Domain.Results;

public enum ErrorType
{
	Failure = 0,
	Validation,
	Problem,
	NotFound,
	Conflict,
	Unauthorized,
}
