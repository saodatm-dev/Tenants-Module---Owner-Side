using Core.Domain.Providers;

namespace Core.Infrastructure.Authentication;

internal sealed class DateTimeProvider : IDateTimeProvider
{
	public DateTime UtcNow => DateTime.UtcNow;
	public DateTime TashkentTime => DateTime.UtcNow.AddHours(5);
}
