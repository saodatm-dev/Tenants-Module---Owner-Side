using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstates.Block;

internal sealed class BlockModerationRealEstateCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<BlockModerationRealEstateCommand, Guid>
{
	public async Task<Result<Guid>> Handle(BlockModerationRealEstateCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstates
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(BlockModerationRealEstateCommand.Id)));

		if (maybeItem.IsBlocked())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(BlockModerationRealEstateCommand.Id)));

		dbContext.RealEstates.Update(maybeItem.Block());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
