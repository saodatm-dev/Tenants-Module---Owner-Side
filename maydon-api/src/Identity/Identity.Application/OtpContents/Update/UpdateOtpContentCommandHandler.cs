using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.OtpContents;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.OtpContents.Update;

internal sealed class UpdateOtpContentCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<UpdateOtpContentCommand>
{
	public async Task<Result> Handle(UpdateOtpContentCommand command, CancellationToken cancellationToken)
	{
		var maybeItems = await dbContext.OtpContents
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.OtpType == command.OtpType)
			.ToListAsync(cancellationToken);

		if (!maybeItems.Any())
			return Result.Failure<Guid>(sharedViewLocalizer.OtpContentNotFound(nameof(UpdateOtpContentCommand.OtpType)));

		foreach (var translate in command.Translates)
		{
			var existingTranslate = maybeItems.FirstOrDefault(item => item.LanguageId == translate.LanguageId);
			if (existingTranslate is not null)
			{
				existingTranslate.Update(
					translate.LanguageId,
					translate.LanguageShortCode,
					translate.Value);
			}
			else
			{
				maybeItems.Add(new OtpContent(
					command.OtpType,
					translate.LanguageId,
					translate.LanguageShortCode,
					translate.Value));
			}
		}

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
