using Core.Domain.Languages;
using Identity.Domain.Otps;

namespace Identity.Application.OtpContents.GetById;

public sealed record GetOtpContentByIdQueryResponse(
	OtpType OtpType,
	IEnumerable<LanguageValue> Translates);
