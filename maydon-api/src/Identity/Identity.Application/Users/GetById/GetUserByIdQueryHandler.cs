using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.GetById;

internal sealed class GetUserByIdQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>
{
	public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		if (!executionContextProvider.IsIndividual)
			return Result<GetUserByIdResponse>.None;

		var user = await dbContext.Users
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Select(item => new GetUserByIdResponse(
				item.Id,
				item.FirstName,
				item.LastName,
				item.MiddleName,
				item.PhoneNumber,
				item.ObjectName))
			.FirstOrDefaultAsync(cancellationToken);

		if (user is null)
			return Result<GetUserByIdResponse>.None;

		var resolvedPhoto = await fileUrlResolver.ResolveUrlAsync(user.Photo, cancellationToken);
		return user with { Photo = resolvedPhoto };
	}
}

