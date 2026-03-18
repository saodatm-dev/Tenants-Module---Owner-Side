using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstates.Reject;

internal sealed class RejectModerationRealEstateCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RejectModerationRealEstateCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RejectModerationRealEstateCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstates
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(RejectModerationRealEstateCommand.Id)));

		if (maybeItem.IsReject())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(RejectModerationRealEstateCommand.Id)));

		dbContext.RealEstates.Update(maybeItem.Reject(command.Reason));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
