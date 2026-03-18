using Core.Domain.Results;

namespace Core.Domain.Exceptions;

public sealed class AuthorizationException : Exception
{
	public AuthorizationException(string parameterName, string message) : base("Unauthorized user.") =>
		Errors = new Error[] { new Error(parameterName, message, ErrorType.Failure) };

	public IReadOnlyCollection<Error> Errors { get; }
}
