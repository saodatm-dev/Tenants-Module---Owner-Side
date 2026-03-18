using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.Profile;

internal sealed class ProfileQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<ProfileQuery, ProfileResponse>
{
	public async Task<Result<ProfileResponse>> Handle(ProfileQuery request, CancellationToken cancellationToken)
	{
		ProfileResponse? profile;

		if (executionContextProvider.IsIndividual)
			profile = await dbContext.Users
				.AsNoTracking()
				.Where(item => item.Id == executionContextProvider.UserId)
				.Select(item => new ProfileResponse(
					item.Id,
					true,
					item.PhoneNumber,
					item.FirstName,
					item.LastName,
					item.MiddleName,
					item.ObjectName,
					item.IsVerified,
					dbContext.GetPermissionNamesByRoleId(executionContextProvider.RoleId)))
				.FirstOrDefaultAsync(cancellationToken);
		else
			profile = await dbContext.CompanyUsers
				.AsNoTracking()
				.Where(item => item.CompanyId == executionContextProvider.TenantId && item.UserId == executionContextProvider.UserId)
				.Include(item => item.Company)
				.Include(item => item.User)
				.Select(item => new ProfileResponse(
						item.Id,
						false,
						item.User.PhoneNumber,
						item.User.FirstName,
						item.User.LastName,
						item.User.MiddleName,
						item.User.ObjectName,
						item.User.IsVerified,
						dbContext.GetPermissionNamesByRoleId(executionContextProvider.RoleId),
						item.Company.Name,
						item.Company.Tin))
				.FirstOrDefaultAsync(cancellationToken);

		if (profile is null)
			return Result<ProfileResponse>.None;

		var resolvedPhoto = await fileUrlResolver.ResolveUrlAsync(profile.Photo, cancellationToken);
		return profile with { Photo = resolvedPhoto };
	}
}
