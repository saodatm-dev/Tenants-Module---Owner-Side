using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.OtpContents;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.OtpContents.Create;

internal sealed class CreateOtpContentCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<CreateOtpContentCommand>
{
	public async Task<Result> Handle(CreateOtpContentCommand command, CancellationToken cancellationToken)
	{
		if (await dbContext.OtpContents
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.AnyAsync(item => item.OtpType == command.OtpType, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateOtpContentCommand.OtpType)));

		await dbContext.OtpContents.AddRangeAsync(
			command.Translates.Select(item =>
				new OtpContent(
					command.OtpType,
					item.LanguageId,
					item.LanguageShortCode,
					item.Value)
			), cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
