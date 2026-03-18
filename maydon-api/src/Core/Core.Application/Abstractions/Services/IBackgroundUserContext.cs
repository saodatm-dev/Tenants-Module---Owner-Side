namespace Core.Application.Abstractions.Services;

/// <summary>
/// Settable user context for background tasks where HttpContext is not available.
/// </summary>
public interface IBackgroundUserContext
{
	Guid? UserId { get; set; }
	void SetUser(Guid? userId);
	void Clear();
}
