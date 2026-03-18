using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Authentication.Registration.CheckPhoneNumber;

internal sealed class CheckPhoneNumberCommandHandler(IIdentityDbContext dbContext) : ICommandHandler<CheckPhoneNumberCommand>
{
	public async Task<Result> Handle(CheckPhoneNumberCommand command, CancellationToken cancellationToken)
	{
		var user = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(item => item.PhoneNumber == command.PhoneNumber, cancellationToken);

		return user is null ? Result.None : Result.Success();
	}
}
