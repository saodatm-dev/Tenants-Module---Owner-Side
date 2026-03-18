using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;
using EntityFrameworkCore.EncryptColumn.Attribute;
using Identity.Domain.Otps;

namespace Identity.Domain.OtpContents;

[Table("otp_contents", Schema = AssemblyReference.Instance)]
public sealed class OtpContent : Entity
{
	private OtpContent() { }
	public OtpContent(
		OtpType otpType,
		Guid languageId,
		string languageShortCode,
		string content) : base()
	{
		this.OtpType = otpType;
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Content = content;
	}
	public OtpType OtpType { get; private set; }
	public Guid LanguageId { get; private set; }
	[Required]
	[MaxLength(10)]
	public string LanguageShortCode { get; private set; }
	[Required]
	[MaxLength(500)]
	[EncryptColumn]
	public string Content { get; private set; }
	public OtpContent Update(
		Guid languageId,
		string languageShortCode,
		string content)
	{
		this.LanguageId = languageId;
		this.LanguageShortCode = languageShortCode;
		this.Content = content;
		return this;
	}
}
