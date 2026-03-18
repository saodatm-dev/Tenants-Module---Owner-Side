using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.Invitations.Get;

public sealed record GetInvitationsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetInvitationsResponse>(Page, PageSize);
