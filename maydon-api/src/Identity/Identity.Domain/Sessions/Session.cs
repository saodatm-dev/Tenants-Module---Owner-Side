using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Identity.Domain.Accounts;

namespace Identity.Domain.Sessions;

[Table("sessions", Schema = AssemblyReference.Instance)]
public sealed class Session : Entity
{
	private Session() { }
	public Session(
		Guid accountId,
		string refreshToken,
		DateTime refreshTokenExpiryTime,
		string deviceInfo,
		string ipAddress) : base()
	{
		this.AccountId = accountId;
		this.RefreshToken = refreshToken;
		this.RefreshTokenExpiryTime = refreshTokenExpiryTime;
		this.DeviceInfo = deviceInfo;
		this.IpAddress = ipAddress;
	}

	public Guid AccountId { get; private set; }
	[EncryptColumn]
	[Required]
	[MaxLength(500)]
	public string RefreshToken { get; private set; }
	public DateTime RefreshTokenExpiryTime { get; private set; }
	public bool IsTerminated { get; private set; }
	[EncryptColumn]
	[MaxLength(500)]
	public string DeviceInfo { get; private set; }
	[EncryptColumn]
	[MaxLength(50)]
	public string IpAddress { get; private set; }
	public Account Account { get; private set; }
	public bool IsExpired(DateTime utcNow) => this.RefreshTokenExpiryTime <= utcNow;

	public Session Update(
		string refreshToken,
		DateTime refreshTokenExpiredTime,
		string? ipAddress = null,
		string? deviceInfo = null)
	{
		this.RefreshToken = refreshToken;
		this.RefreshTokenExpiryTime = refreshTokenExpiredTime;
		if (ipAddress is not null) this.IpAddress = ipAddress;
		if (deviceInfo is not null) this.DeviceInfo = deviceInfo;
		return this;
	}
	public Session Terminate()
	{
		IsTerminated = true;
		return this;
	}
}
