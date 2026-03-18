namespace Core.Domain.Exceptions;

public sealed class RetryableJobException : Exception
{
	public RetryableJobException(string message, Exception? inner = null)
		: base(message, inner) { }
}
