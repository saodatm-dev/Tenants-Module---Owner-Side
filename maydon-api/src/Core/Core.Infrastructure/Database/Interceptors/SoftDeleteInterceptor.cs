using Core.Domain.Entities;
using Core.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Infrastructure.Database.Interceptors;

public sealed class SoftDeleteInterceptor(IDateTimeProvider dateTime) : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default)
	{
		if (eventData.Context is not null)
			await SoftDeleteEntitiesAsync(eventData.Context);

		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private ValueTask SoftDeleteEntitiesAsync(DbContext context)
	{
		foreach (EntityEntry<ISoftDeleteEntity> entry in context
				.ChangeTracker.Entries<ISoftDeleteEntity>()
				.ToList())
		{
			if (entry.State != EntityState.Deleted)
				continue;

			entry.Property(nameof(ISoftDeleteEntity.DeletedAt)).CurrentValue = dateTime.UtcNow;

			entry.Property(nameof(ISoftDeleteEntity.IsDeleted)).CurrentValue = true;

			entry.State = EntityState.Modified;

			UpdateDeletedEntityEntryReferencesToUnchanged(entry);
		}

		return ValueTask.CompletedTask;
	}

	/// <summary>
	/// Updates the specified entity entry's referenced entries in the Deleted state to the modified state.
	/// This method is recursive.
	/// </summary>
	/// <param name="entityEntry">The entity entry.</param>
	private static void UpdateDeletedEntityEntryReferencesToUnchanged(EntityEntry entityEntry)
	{
		if (!entityEntry.References.Any())
			return;

		foreach (ReferenceEntry referenceEntry in entityEntry.References.Where(r => r.TargetEntry?.State == EntityState.Deleted))
		{
			if (referenceEntry.TargetEntry is null)
				continue;

			referenceEntry.TargetEntry.State = EntityState.Unchanged;

			UpdateDeletedEntityEntryReferencesToUnchanged(referenceEntry.TargetEntry);
		}
	}
}
