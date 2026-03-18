using Core.Domain.Results;

namespace Core.Domain.Exceptions;

public sealed class AccessDeniedException : Exception
{
	public AccessDeniedException(string parameterName, string message) : base("Access denied.") =>
		Errors = new Error[] { new Error(parameterName, message, ErrorType.Failure) };

	public IReadOnlyCollection<Error> Errors { get; }
}
