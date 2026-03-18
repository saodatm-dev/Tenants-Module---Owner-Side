using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using Core.Domain.Results;
using EntityFrameworkCore.EncryptColumn.Attribute;

namespace Identity.Domain.Otps;

[Table("otps", Schema = AssemblyReference.Instance)]
public sealed class Otp : Entity
{
	private const int SendTimeInMinute = 1;
	private const int BlockTimeInMinute = 30;
	private const ushort TriesCount = 1;
	private const ushort MaxTriesCount = 4;
	private const ushort MaxSendMessageCount = 3;

	private Otp() { }
	public Otp(
		string phoneNumber,
		string code,
		ushort sendMessageCount = 1) : base()
	{
		this.PhoneNumber = phoneNumber;
		this.Code = code;
		this.SentMessageCount = sendMessageCount;
		this.NextAvailableTime = null;
	}
	[EncryptColumn]
	[Required]
	[MaxLength(20)]
	public string PhoneNumber { get; private set; }
	[EncryptColumn]
	[Required]
	[MaxLength(10)]
	public string Code { get; private set; }
	public DateTime? NextAvailableTime { get; private set; }
	public ushort SentMessageCount { get; private set; } = 1;
	public ushort Tries { get; private set; } = TriesCount;
	public ushort MaxTries { get; private set; } = MaxTriesCount;
	public OtpStatus Status { get; private set; }
	public Result Try(
		string code,
		DateTime currentDateTime)
	{
		// block to 30 minutes
		if (Tries == MaxTries)
		{
			Status = OtpStatus.Block;
			NextAvailableTime = currentDateTime.AddMinutes(BlockTimeInMinute);
			return Result.Failure(Error.None);
		}

		if (!string.Equals(this.Code, code, StringComparison.OrdinalIgnoreCase))
		{
			Tries++;
			return Result.Failure(Error.None);
		}

		return Result.Success();
	}
	public OtpStatus Resend(
		string code,
		DateTime currentDateTime)
	{
		if (Status == OtpStatus.Block && NextAvailableTime != null && NextAvailableTime > currentDateTime)
			return OtpStatus.Block;

		if (NextAvailableTime != null && NextAvailableTime > currentDateTime)
			return OtpStatus.Waiting;

		if (Status == OtpStatus.Block && NextAvailableTime != null && NextAvailableTime < currentDateTime)
		{
			this.Reset();
		}

		this.Code = code;
		this.NextAvailableTime = currentDateTime.AddMinutes(SendTimeInMinute);

		return OtpStatus.Active;
	}
	public Otp Reset()
	{
		this.Status = OtpStatus.Active;
		this.NextAvailableTime = null;
		return this;
	}
	public Otp Sent(DateTime currentDateTime)
	{
		this.Status = OtpStatus.Waiting;
		NextAvailableTime = currentDateTime.AddMinutes(SendTimeInMinute);
		return this;
	}
	public Otp Block(DateTime currentDateTime)
	{
		this.Status = OtpStatus.Block;
		NextAvailableTime = currentDateTime.AddMinutes(BlockTimeInMinute);
		return this;
	}
	public Otp Received()
	{
		this.Status = OtpStatus.Received;
		this.NextAvailableTime = null;
		return this;
	}
	public Otp NotApplied()
	{
		this.Status = OtpStatus.NotApplied;
		return this;
	}
	public OtpStatus IsAvailable(DateTime currentDateTime)
	{
		if (this.SentMessageCount >= MaxSendMessageCount && NextAvailableTime is not null && NextAvailableTime > currentDateTime && Status != OtpStatus.Block)
		{
			this.Block(currentDateTime);
			return Status;
		}

		if (Status == OtpStatus.Block && NextAvailableTime is not null && NextAvailableTime > currentDateTime)
			return Status;

		if (NextAvailableTime is null || NextAvailableTime < currentDateTime)
			return OtpStatus.Active;

		return Status;
	}
}
