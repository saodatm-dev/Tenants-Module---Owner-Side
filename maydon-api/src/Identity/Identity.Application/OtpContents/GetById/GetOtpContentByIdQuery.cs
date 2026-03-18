using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;
using Identity.Domain.Otps;

namespace Identity.Application.OtpContents.GetById;

public sealed record GetOtpContentByIdQuery([property: Required] OtpType OtpType) : IQuery<GetOtpContentByIdQueryResponse>;
