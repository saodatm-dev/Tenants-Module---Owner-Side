using Identity.Domain.Invitations;

namespace Identity.Application.Invitations.GetById;

public sealed record GetInvitationByIdResponse(
	Guid Id,
	string PhoneNumber,
	string FullName,
	string RoleName,
	DateTime ExpiredTime,
	InvitationStatus Status,
	string? Reason);
