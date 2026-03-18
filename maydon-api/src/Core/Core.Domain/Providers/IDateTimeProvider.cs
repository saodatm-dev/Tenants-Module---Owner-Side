namespace Core.Domain.Providers;

public interface IDateTimeProvider
{
	const short TashkentTimeDifference = 5;
	DateTime UtcNow { get; }
}
