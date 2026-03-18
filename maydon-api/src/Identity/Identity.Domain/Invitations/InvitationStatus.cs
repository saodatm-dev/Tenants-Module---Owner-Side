namespace Identity.Domain.Invitations;

public enum InvitationStatus
{
	Sent = 0,
	Received,
	Accepted,
	Canceled,
	Rejected
}
