namespace Core.Infrastructure.Authorization;

public interface ISessionProvider
{
	public Task<bool> IsActual(Guid sessionId, Guid accountId);
}
