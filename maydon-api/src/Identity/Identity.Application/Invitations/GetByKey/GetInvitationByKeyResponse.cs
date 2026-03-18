namespace Identity.Application.Invitations.GetByKey;

public sealed record GetInvitationByKeyResponse(
	Guid Id,
	string Content);
