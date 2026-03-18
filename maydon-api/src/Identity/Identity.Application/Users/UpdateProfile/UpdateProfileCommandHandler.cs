using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.UpdateProfile;

internal sealed class UpdateProfileCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<UpdateProfileCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
	{
		var user = await dbContext.Users
			.FirstOrDefaultAsync(item => item.Id == executionContextProvider.UserId, cancellationToken);

		if (user is null)
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(User)));

		if (user.IsVerified)
			return Result.Failure<Guid>(sharedViewLocalizer.UserIsVerified(nameof(User)));

		dbContext.Users.Update(
			user.UpdateProfile(
				firstName: command.FirstName,
				lastName: command.LastName,
				middleName: command.MiddleName,
				objectName: command.ProfilePicture));

		await dbContext.SaveChangesAsync(cancellationToken);

		return user.Id;
	}
}
