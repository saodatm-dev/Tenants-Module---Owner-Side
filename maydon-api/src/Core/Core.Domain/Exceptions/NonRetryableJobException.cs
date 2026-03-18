namespace Core.Domain.Exceptions;

public sealed class NonRetryableJobException : Exception
{
	public NonRetryableJobException(string message)
		: base(message) { }
}
