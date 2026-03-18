using Core.Application.Abstractions.Authentication;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Core.Infrastructure.Database.Interceptors;

public sealed class ModifiableInterceptor(IExecutionContextProvider executionContextProvider) : SaveChangesInterceptor
{
	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
		DbContextEventData eventData,
		InterceptionResult<int> result,
		CancellationToken cancellationToken = default
	)
	{
		if (eventData.Context is not null)
			await ModifiableEntitiesAsync(eventData.Context);

		return await base.SavingChangesAsync(eventData, result, cancellationToken);
	}

	private ValueTask ModifiableEntitiesAsync(DbContext context)
	{
		foreach (var entry in context.ChangeTracker.Entries<IModifiableEntity>().ToList())
		{
			if (entry.State == EntityState.Added && executionContextProvider.UserId != Guid.Empty)
				entry.Property(nameof(IModifiableEntity.CreatedBy)).CurrentValue = executionContextProvider.UserId;

			if (entry.State == EntityState.Modified && executionContextProvider.UserId != Guid.Empty)
				entry.Property(nameof(IModifiableEntity.UpdatedBy)).CurrentValue = executionContextProvider.UserId;
		}

		return ValueTask.CompletedTask;
	}
}
