using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstates.Accept;

internal sealed class AcceptModerationRealEstateCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<AcceptModerationRealEstateCommand, Guid>
{
	public async Task<Result<Guid>> Handle(AcceptModerationRealEstateCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstates
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(AcceptModerationRealEstateCommand.Id)));

		if (maybeItem.IsAccept())
			return maybeItem.Id;

		// check status 
		if (maybeItem.IsCancel())
			return Result.Failure<Guid>(sharedViewLocalizer.WasCancelledByUser(nameof(AcceptModerationRealEstateCommand.Id)));

		dbContext.RealEstates.Update(maybeItem.Accept());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
