using Core.Application.Abstractions.Services;

namespace Core.Infrastructure.Services;

/// <summary>
/// Scoped implementation of IBackgroundUserContext for background task processing.
/// Allows setting user identity when processing integration events that carry InitiatedBy.
/// </summary>
public class BackgroundUserContext : IBackgroundUserContext
{
    public Guid? UserId { get; set; }

    public void SetUser(Guid? userId)
    {
        UserId = userId;
    }

    public void Clear()
    {
        UserId = null;
    }
}
