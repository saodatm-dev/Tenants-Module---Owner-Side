using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Companies.GetById;

internal sealed class GetCompanyByIdQueryHandler(
	IIdentityDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdResponse>
{
	public async Task<Result<GetCompanyByIdResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
	{
		var company = await dbContext.Companies
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Select(item => new GetCompanyByIdResponse(
				item.Id,
				item.Name,
				item.Tin,
				item.IsVerified,
				item.ObjectName))
			.FirstOrDefaultAsync(cancellationToken);

		if (company is null)
			return Result<GetCompanyByIdResponse>.None;

		var resolvedPhoto = await fileUrlResolver.ResolveUrlAsync(company.Photo, cancellationToken);
		return company with { Photo = resolvedPhoto };
	}
}

