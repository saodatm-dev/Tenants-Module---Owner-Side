using Core.Domain.Entities;
using Core.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Infrastructure.Database.Interceptors;

public sealed class AuditableInterceptor(IDateTimeProvider dateTime) : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		if (eventData.Context is not null)
			await AuditableEntitiesAsync(eventData.Context);

		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private ValueTask AuditableEntitiesAsync(DbContext context)
	{
		foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>().ToList())
		{
			if (entry.State == EntityState.Added)
				entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = dateTime.UtcNow;

			if (entry.State == EntityState.Modified)
				entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = dateTime.UtcNow;
		}

		return ValueTask.CompletedTask;
	}
}
