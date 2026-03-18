using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstates.Cancel;

internal sealed class CancelModerationRealEstateCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CancelModerationRealEstateCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CancelModerationRealEstateCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstates
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(CancelModerationRealEstateCommand.Id)));

		if (maybeItem.IsCancel())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsBlocked())
			return Result.Failure<Guid>(sharedViewLocalizer.WasBlockedByModerator(nameof(CancelModerationRealEstateCommand.Id)));

		if (maybeItem.IsReject())
			return Result.Failure<Guid>(sharedViewLocalizer.WasRejectedByModerator(nameof(CancelModerationRealEstateCommand.Id)));

		if (maybeItem.IsAccept())
			return Result.Failure<Guid>(sharedViewLocalizer.WasAcceptedByModerator(nameof(CancelModerationRealEstateCommand.Id)));

		dbContext.RealEstates.Update(maybeItem.Cancel());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
