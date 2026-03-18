using Identity.Domain.Invitations;

namespace Identity.Application.Invitations.Get;

public sealed record GetInvitationsResponse(
	Guid Id,
	string PhoneNumber,
	string FullName,
	string RoleName,
	DateTime ExpiredTime,
	InvitationStatus Status,
	string? Reason);
