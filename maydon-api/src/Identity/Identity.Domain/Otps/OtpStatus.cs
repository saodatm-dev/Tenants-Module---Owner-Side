namespace Identity.Domain.Otps;

public enum OtpStatus
{
	Active = 0,
	Received,
	Waiting,
	Block,
	NotApplied,
}
